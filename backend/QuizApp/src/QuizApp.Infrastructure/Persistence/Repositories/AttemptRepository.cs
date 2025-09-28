using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Infrastructure.Persistence.Context;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class AttemptRepository(
    QuizDbContext dbContext
) : IAttemptRepository
{
    public async Task AddAsync(Attempt attempt, CancellationToken cancellationToken)
    {
        await dbContext
            .Attempts
            .AddAsync(attempt, cancellationToken)
            .AsTask();
    }

    public async Task<Attempt?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext
            .Attempts
            .Include(a => a.Quiz)
            .Include(a => a.Items)
                .ThenInclude(i => i.SelectedChoices)
            .Include(a => a.Items)
                .ThenInclude(i => i.TextAnswer)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Attempt?> FindActiveForUserQuizAsync(Guid userId, Guid quizId, CancellationToken cancellationToken)
    {
        return await dbContext
            .Attempts
            .Include(a => a.Items)
            .FirstOrDefaultAsync(a =>
                a.UserId == userId &&
                a.QuizId == quizId &&
                a.Status == Domain.Enums.AttemptStatus.InProgress, cancellationToken);
    }

    public IQueryable<Attempt> Query()
    {
        return dbContext
            .Attempts
            .AsQueryable();
    }
}
