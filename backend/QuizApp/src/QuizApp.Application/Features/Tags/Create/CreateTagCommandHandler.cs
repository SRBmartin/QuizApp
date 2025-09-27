using AutoMapper;
using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Tags;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Tags.Create;

public class CreateTagCommandHandler (
    ITagRepository tagRepository,
    IUnitOfWork uow,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<CreateTagCommand, Result<TagDto>>
{
    public async Task<Result<TagDto>> Handle(CreateTagCommand command, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<TagDto>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var tag = Tag.Create(Guid.NewGuid(), command.Name, currentUser.Id.Value);

        await tagRepository.AddAsync(tag, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<TagDto>(tag);
        
        return Result<TagDto>.Success(dto);
    }
}
