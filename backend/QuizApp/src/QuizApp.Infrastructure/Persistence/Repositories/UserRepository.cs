using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Infrastructure.Persistence.Context;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class UserRepository (
    QuizDbContext dbContext    
) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user,cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await dbContext
            .Users
            .AsNoTracking()
            .AnyAsync(t => t.Email.Equals(email), cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await dbContext
            .Users
            .AsNoTracking()
            .AnyAsync(t => t.Username.Equals(username), cancellationToken); 
    }
}
