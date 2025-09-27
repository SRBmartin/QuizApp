using AutoMapper;
using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Tags;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Tags.Update;

public class UpdateTagCommandHandler (
    ITagRepository tagRepository,
    IUnitOfWork uow,
    IMapper mapper
) : IRequestHandler<UpdateTagCommand, Result<TagDto>>
{
    public async Task<Result<TagDto>> Handle(UpdateTagCommand command, CancellationToken cancellationToken)
    {
        var tag = await tagRepository.FindByIdAsync(command.Id, cancellationToken);
        if (tag is null)
            return Result<TagDto>.Failure(new Error("tag.not_found", "Tag not found."));

        tag.Update(command.Name);

        tagRepository.Update(tag);
        await uow.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<TagDto>(tag);

        return Result<TagDto>.Success(dto);
    }
}
