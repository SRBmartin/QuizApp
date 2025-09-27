using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Tags.Delete;

public class DeleteTagCommandHandler (
    ITagRepository tagRepository,
    IQuizTagRepository quizTagRepository,
    IUnitOfWork uow
) : IRequestHandler<DeleteTagCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteTagCommand command, CancellationToken cancellationToken)
    {
        var tag = await tagRepository.FindByIdAsync(command.Id, cancellationToken);
        if (tag is null)
            return Result<Unit>.Failure(new Error("tag.not_found", "Tag not found."));

        if (await quizTagRepository.ExistsForTagAsync(tag.Id, cancellationToken))
        {
            return Result<Unit>.Failure(new Error("tag.conflict", "Tag is connected to some quizes and can't be deleted."));
        }

        tag.Delete();

        tagRepository.Remove(tag);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
