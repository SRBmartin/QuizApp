using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Repositories.UoW;
using QuizApp.Infrastructure.Persistence.Context;

namespace QuizApp.Infrastructure.Persistence.Repositories.UoW;

public class UnitOfWork (
    QuizDbContext dbContext    
) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
