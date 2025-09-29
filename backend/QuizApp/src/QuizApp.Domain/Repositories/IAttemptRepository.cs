using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Repositories;

public interface IAttemptRepository
{
    Task AddAsync(Attempt attempt, CancellationToken cancellationToken);
    Task<Attempt?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Attempt?> FindActiveForUserQuizAsync(Guid userId, Guid quizId, CancellationToken cancellationToken);
    IQueryable<Attempt> Query();
}