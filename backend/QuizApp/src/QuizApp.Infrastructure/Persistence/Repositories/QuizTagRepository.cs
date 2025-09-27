using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Repositories;
using QuizApp.Infrastructure.Persistence.Context;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuizTagRepository (
    QuizDbContext dbContext    
) : IQuizTagRepository
{
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
}
