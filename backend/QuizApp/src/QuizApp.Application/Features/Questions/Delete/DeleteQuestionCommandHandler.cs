using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Result;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Questions.Delete;

public class DeleteQuestionCommandHandler (
    IQuizQuestionRepository quizQuestionRepository,
    IQuizRepository quizRepository,
    IUnitOfWork uow,
    ICurrentUser currentUser
) : IRequestHandler<DeleteQuestionCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteQuestionCommand command, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<Unit>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var question = await quizQuestionRepository.FindByIdAsync(command.Id, cancellationToken);
        if (question is null)
            return Result<Unit>.Failure(new Error("question.not_found", "Question not found."));

        var quiz = await quizRepository.FindByIdAsync(question.QuizId, cancellationToken);
        if (quiz is not null && quiz.IsPublished)
            return Result<Unit>.Failure(new Error("quiz.published", "Quiz is published so questions cannot be deleted."));

        question.Delete();

        await uow.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }

}
