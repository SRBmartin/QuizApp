using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;

namespace QuizApp.Domain.Repositories;

public interface IQuizRepository
{
    Task AddAsync(Quiz quiz, CancellationToken cancellationToken);
    Task<Quiz?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> ExistsByNameExceptIdAsync(string name, Guid exceptId, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<int> CountAsync(bool includeUnpublished, Guid? tagId, QuizLevel? difficulty, string? search, CancellationToken cancellationToken);
    Task<List<Quiz>> ListAsync(int skip, int take, bool includeUnpublished, Guid? tagId, QuizLevel? difficulty, string? search, CancellationToken cancellationToken);
}
