namespace QuizApp.Api.Contracts.Quizzes;

public class UpdateQuizTagsRequest
{
    public List<Guid> TagIds { get; set; } = new();
}
