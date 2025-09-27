using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Infrastructure.Persistence.Context;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class TagRepository(
    QuizDbContext dbContext
) : ITagRepository
{
    public async Task AddAsync(Tag tag, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(tag, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await dbContext
            .Tags
            .AnyAsync(t => t.Name.ToLower().Equals(name.ToLower()), cancellationToken);
    }

    public async Task<bool> ExistsByNameExceptAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        return await dbContext
            .Tags
            .AnyAsync(t =>
                t.Id != id && t.Name.ToLower().Equals(name.ToLower()),
                cancellationToken
            );
    }

    public async Task<Tag?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext
            .Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Tag>> ListAsync(int? skip, int? take, CancellationToken cancellationToken)
    {
        var q = dbContext
            .Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .AsQueryable();

        if (skip is not null) q = q.Skip(skip.Value);
        if (take is not null) q = q.Take(take.Value);

        return await q.ToListAsync(cancellationToken);
    }

    public void Remove(Tag tag)
    {
        dbContext
            .Tags
            .Update(tag); //logic delete
    }

    public void Update(Tag tag)
    {
        dbContext
            .Tags
            .Update(tag);
    }
}
