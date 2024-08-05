
using DbUp;
using Microsoft.Extensions.Configuration;
using QandA.Data.Models;
using QandA.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using QandA.Authorization;
using Microsoft.IdentityModel.Logging;
using Keycloak.AuthServices.Authentication;
using System.Security.Claims;
using QandA.ExtentionMethods;

namespace QandA
{
    public partial class Program
    {

        private const string corsPolicy = "CorsPolicy";
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var configuration = builder.Configuration;
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            EnsureDatabase.For.SqlDatabase(connectionString);
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString, null)
                .WithScriptsEmbeddedInAssembly(System.Reflection.Assembly.GetExecutingAssembly())
                .WithTransaction()
                .Build();

            if (upgrader.IsUpgradeRequired())
            {
                var result = upgrader.PerformUpgrade();
                if (result.Successful == false)
                {
                    throw new Exception($"Не удалось обновить базу данных. Ошибка: {result.Error}");
                }
            }

            var services = builder.Services;
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddScoped<IDataRepository, DataRepository>();
            services.AddMemoryCache();
            services.AddSingleton<IQuestionCache, QuestionCache>();
            services.AddHttpClient();
            services.AddScoped<IAuthorizationHandler, MustBeQuestionAuthorHandler>();
            services.AddHttpContextAccessor();

            services.AddCors(options =>
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(configuration["Frontend"])));

            if(configuration.IsAuthService(AuthentificationService.Auth0))
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                      JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme =
                      JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.Authority = configuration["Auth0:Authority"];
                    options.Audience = configuration["Auth0:Audience"];
                });

            }
            else if(configuration.IsAuthService(AuthentificationService.KeyCloak))
            {
                services.AddKeycloakWebApiAuthentication(builder.Configuration);
            }
            else
            {
                throw new NotSupportedException($"\"{configuration[nameof(AuthentificationService)]}\" error authService");
            }


            services.AddAuthorization(options => options
                .AddPolicy("MustBeQuestionAuthor", policy => policy
                .Requirements
                    .Add(new MustBeQuestionAuthorRequirement()))
            );

            services.AddCors(options =>
                options.AddPolicy(corsPolicy, builder =>
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(configuration["Frontend"])));

            var app = builder.Build();
            
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHttpsRedirection();
            }
            
            app.UseCors(corsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }


}
