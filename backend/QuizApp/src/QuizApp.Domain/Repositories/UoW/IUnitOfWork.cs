namespace QuizApp.Domain.Repositories.UoW;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
