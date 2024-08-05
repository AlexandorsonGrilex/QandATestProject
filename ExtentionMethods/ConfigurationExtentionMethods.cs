using Microsoft.AspNetCore.Authentication;
using static QandA.Program;

namespace QandA.ExtentionMethods
{
    public static class ConfigurationExtentionMethods
    {
        /// <summary>
        /// Настройка сервиса аутентификации в конфиругации.
        /// Раздел AuthentificationService
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="authService"></param>
        /// <returns></returns>
        public static bool IsAuthService(this IConfiguration configuration, AuthentificationService authService)
        {
            var intValue = (int)configuration.GetValue(typeof(int), nameof(AuthentificationService));
            return intValue == (int)authService;
        }
    }
}
