namespace QuizApp.Domain.Repositories;

public interface IQuizTagRepository
{
    Task<bool> ExistsForTagAsync(Guid tagId, CancellationToken cancellationToken);
}
