using QuizApp.Domain.Entities;

namespace QuizApp.Domain.Repositories;

public interface IQuizQuestionRepository 
{
    Task AddAsync(QuizQuestion question, CancellationToken cancellationToken);
    Task<QuizQuestion?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
}
