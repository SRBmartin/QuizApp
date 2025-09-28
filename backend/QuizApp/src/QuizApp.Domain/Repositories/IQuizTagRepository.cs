using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Repositories;

public interface IQuizTagRepository
{
    Task<bool> ExistsForTagAsync(Guid tagId, CancellationToken cancellationToken);
    Task<List<Guid>> GetTagIdsForQuizAsync(Guid quizId, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<QuizTag> items, CancellationToken cancellationToken);
    Task RemoveRangeAsync(Guid quizId, IEnumerable<Guid> tagIds, CancellationToken cancellationToken);
    Task<List<Tag>> GetTagsForQuizAsync(Guid quizId, CancellationToken cancellationToken);
    Task<Dictionary<Guid, List<Tag>>> GetTagsForQuizzesAsync(IEnumerable<Guid> quizIds, CancellationToken cancellationToken);
}
