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

    public async Task<Quiz?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext
            .Quizzes
            .Include(t => t.Questions)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Quiz>> ListAsync(int skip, int take, CancellationToken cancellationToken)
    {
        return await dbContext
            .Quizzes
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

}
