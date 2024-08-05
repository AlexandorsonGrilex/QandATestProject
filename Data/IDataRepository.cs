using QandA.Data.Models;

namespace QandA.Data
{
    public interface IDataRepository
    {
        Task<IEnumerable<QuestionGetManyResponse>> GetQuestions();

        Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsWithAnswers();

        IEnumerable<QuestionGetManyResponse> GetQuestionsBySearch(string search);

        Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsBySearchWithPaging(string search, int pageNumber, int pageSize);

        IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions();
        
        Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestionsAsync();

        Task<QuestionGetSingleResponse?> GetQuestion(int questionId);

        bool QuestionExists(int questionId);

        AnswerGetResponse? GetAnswer(int answerId);

        Task<QuestionGetSingleResponse> PostQuestion(QuestionPostFullRequest question);
        
        Task<QuestionGetSingleResponse> PostQuestionAsync(QuestionPostFullRequest question);

        Task<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest question);

        void DeleteQuestion(int questionId);

        AnswerGetResponse PostAnswer(AnswerPostFullRequest answer);
        
    }
}