using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Repositories;

public interface IQuizRepository
{
    Task AddAsync(Quiz quiz, CancellationToken cancellationToken);
    Task<Quiz?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> ExistsByNameExceptIdAsync(string name, Guid exceptId, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<List<Quiz>> ListAsync(int skip, int take, bool includeUnpublished, CancellationToken cancellationToken);
}
