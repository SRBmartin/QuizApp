using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Quizes.Delete;

public class DeleteQuizCommandHandler (
    IQuizRepository quizRepository,
    IUnitOfWork uow
) : IRequestHandler<DeleteQuizCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteQuizCommand command, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.FindByIdAsync(command.Id, cancellationToken);
        if (quiz is null)
            return Result<Unit>.Failure(new Error("quiz.not_found", "Quiz not found."));

        quiz.Delete();
        await uow.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }

}
