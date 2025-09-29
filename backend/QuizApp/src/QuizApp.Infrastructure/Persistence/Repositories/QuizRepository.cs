using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
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
            .Include(t => t.Questions)
                .ThenInclude(c => c.TextAnswer)
            .Include(t => t.QuizTags)
                .ThenInclude(c => c.Tag)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Quiz>> ListAsync(
        int skip,
        int take,
        bool includeUnpublished,
        Guid? tagId,
        QuizLevel? difficulty,
        string? search,
        CancellationToken cancellationToken
    )
    {
        var query = dbContext
            .Quizzes
            .Include(t => t.QuizTags)
                .ThenInclude(c => c.Tag)
            .OrderByDescending(t => t.CreatedAt)
            .AsQueryable();

        query = ApplyFilters(query, includeUnpublished, tagId, difficulty, search);

        return await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(bool includeUnpublished, Guid? tagId, QuizLevel? difficulty, string? search, CancellationToken cancellationToken)
    {
        var query = dbContext
            .Quizzes
            .AsQueryable();

        query = ApplyFilters(query, includeUnpublished, tagId, difficulty, search);

        return await query.CountAsync(cancellationToken);
    }

    public IQueryable<Quiz> Query()
    {
        return dbContext.Quizzes.AsQueryable();
    }

    #region Helpers

    private static IQueryable<Quiz> ApplyFilters(
        IQueryable<Quiz> query,
        bool includeUnpublished,
        Guid? tagId,
        QuizLevel? difficulty,
        string? search)
    {
        if (!includeUnpublished)
        {
            query = query.Where(t => t.IsPublished);
        }

        if (tagId.HasValue)
        {
            query = query.Where(t => t.QuizTags.Any(qt => qt.TagId == tagId.Value));
        }

        if (difficulty.HasValue)
        {
            query = query.Where(t => t.DifficultyLevel == difficulty.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            
            query = query.Where(t =>
                EF.Functions.Like(t.Name.ToLower(), $"%{s}%") ||
                (t.Description != null && EF.Functions.Like(t.Description.ToLower(), $"%{s}%"))
            );
        }

        return query;
    }

    #endregion

}
