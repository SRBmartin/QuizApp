using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Repositories;

public interface ITagRepository
{
    Task AddAsync(Tag tag, CancellationToken cancellationToken);
    void Update(Tag tag);
    void Remove(Tag tag);

    Task<Tag?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<bool> ExistsByNameExceptAsync(Guid id, string name, CancellationToken cancellationToken);

    Task<int> CountExistingAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    Task<List<Tag>> ListByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<Tag>> ListAsync(int? skip, int? take, CancellationToken cancellationToken);
}
