using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Infrastructure.Persistence.Context;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuizRepository(
    QuizDbContext dbContext
) : IQuizRepository
{
    public async Task AddAsync(Quiz quiz, CancellationToken cancellationToken)
    {
        await dbContext
            .Quizzes
            .AddAsync(quiz, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await dbContext
            .Quizzes
            .CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await dbContext
            .Quizzes
            .AsNoTracking()
            .AnyAsync(
                t => t.Name == name,
                cancellationToken
            );
    }

    public async Task<bool> ExistsByNameExceptIdAsync(string name, Guid exceptId, CancellationToken cancellationToken)
    {
        return await dbContext
            .Quizzes
            .AsNoTracking()
            .AnyAsync(
                t => t.Name == name && t.Id != exceptId,
                cancellationToken
            );
    }

    public async Task<Quiz?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext
            .Quizzes
            .Include(t => t.Questions)
                .ThenInclude(c => c.Choices)
            .Include(t => t.QuizTags)
                .ThenInclude(c => c.Tag)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Quiz>> ListAsync(int skip, int take, bool includeUnpublished, CancellationToken cancellationToken)
    {
        var query = dbContext
            .Quizzes
            .Include(t => t.QuizTags)
                .ThenInclude(c => c.Tag)
            .OrderByDescending(t => t.CreatedAt)
            .AsQueryable();

        if (!includeUnpublished)
        {
            query = query.Where(t => t.IsPublished);
        }

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

}
