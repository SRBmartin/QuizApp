using AutoMapper;
using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Tags;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Quizes.UpdateTags;

public class UpdateQuizTagsCommandHandler (
    IQuizRepository quizRepository,
    ITagRepository tagRepository,
    IQuizTagRepository quizTagRepository,
    IUnitOfWork uow,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<UpdateQuizTagsCommand, Result<IReadOnlyList<TagDto>>>
{
    public async Task<Result<IReadOnlyList<TagDto>>> Handle(UpdateQuizTagsCommand command, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<IReadOnlyList<TagDto>>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var quiz = await quizRepository.FindByIdAsync(command.QuizId, cancellationToken);
        if (quiz is null)
            return Result<IReadOnlyList<TagDto>>.Failure(new Error("quiz.not_found", "Quiz not found."));

        if (command.TagIds.Count > 0)
        {
            var existingCount = await tagRepository.CountExistingAsync(command.TagIds, cancellationToken);
            if (existingCount != command.TagIds.Distinct().Count())
                return Result<IReadOnlyList<TagDto>>.Failure(new Error("tags.invalid", "One or more tags do not exist."));
        }

        var current = await quizTagRepository.GetTagIdsForQuizAsync(command.QuizId, cancellationToken);
        var desired = command.TagIds.Distinct().ToList();

        var toAdd = desired.Except(current).ToList();
        var toRemove = current.Except(desired).ToList();

        if (toRemove.Count > 0)
        {
            await quizTagRepository.RemoveRangeAsync(command.QuizId, toRemove, cancellationToken);
        }

        if (toAdd.Count > 0)
        {
            var entities = toAdd.Select(tid => QuizTag.Create(command.QuizId, tid));

            await quizTagRepository.AddRangeAsync(entities, cancellationToken);
        }

        await uow.SaveChangesAsync(cancellationToken);

        var updatedTags = await tagRepository.ListByIdsAsync(desired, cancellationToken);
        var dtos = updatedTags.Select(mapper.Map<TagDto>).ToList().AsReadOnly();

        return Result<IReadOnlyList<TagDto>>.Success(dtos);
    }

}
