using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Infrastructure.Persistence.Context;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuizQuestionRepository(
    QuizDbContext dbContext
) : IQuizQuestionRepository
{
    public async Task AddAsync(QuizQuestion question, CancellationToken cancellationToken)
    {
        await dbContext
            .QuizQuestions
            .AddAsync(question, cancellationToken);
    }

    public async Task<QuizQuestion?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext
            .QuizQuestions
            .Include(t => t.Choices)
            .Include(t => t.TextAnswer)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<int> CountByQuizIdAsync(Guid quizId, CancellationToken cancellationToken)
    {
        return await dbContext
            .QuizQuestions
            .CountAsync(t => t.QuizId == quizId, cancellationToken);
    }

}
