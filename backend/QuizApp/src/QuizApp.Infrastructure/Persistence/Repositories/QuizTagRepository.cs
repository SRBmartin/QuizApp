using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Infrastructure.Persistence.Context;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuizTagRepository (
    QuizDbContext dbContext    
) : IQuizTagRepository
{
    public async Task AddRangeAsync(IEnumerable<QuizTag> items, CancellationToken cancellationToken)
    {
        await dbContext
            .QuizTags
            .AddRangeAsync(items, cancellationToken);
    }

    public async Task<bool> ExistsForTagAsync(Guid tagId, CancellationToken cancellationToken)
    {
        return await dbContext
            .QuizTags
            .AsNoTracking()
            .AnyAsync(
                t => t.TagId == tagId,
                cancellationToken
            );
    }

    public async Task<List<Guid>> GetTagIdsForQuizAsync(Guid quizId, CancellationToken cancellationToken)
    {
        return await dbContext
            .QuizTags
            .Where(t => t.QuizId == quizId)
            .Select(t => t.TagId)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveRangeAsync(Guid quizId, IEnumerable<Guid> tagIds, CancellationToken cancellationToken)
    {
        if (!tagIds.Any()) return;

        var toRemove = await dbContext.QuizTags
            .Where(t => t.QuizId == quizId && tagIds.Contains(t.TagId))
            .ToListAsync(cancellationToken);

        if (toRemove.Count > 0)
            dbContext.QuizTags.RemoveRange(toRemove);
    }

    public async Task<List<Tag>> GetTagsForQuizAsync(Guid quizId, CancellationToken cancellationToken)
    {
        return await dbContext.QuizTags
            .Where(qt => qt.QuizId == quizId)
            .Select(qt => qt.Tag)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, List<Tag>>> GetTagsForQuizzesAsync(IEnumerable<Guid> quizIds, CancellationToken cancellationToken)
    {
        var ids = quizIds.Distinct().ToArray();

        var rows = await dbContext
            .QuizTags
            .Where(qt => ids.Contains(qt.QuizId))
            .Select(qt => new { qt.QuizId, qt.Tag })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => x.QuizId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Tag).ToList());
    }

}
