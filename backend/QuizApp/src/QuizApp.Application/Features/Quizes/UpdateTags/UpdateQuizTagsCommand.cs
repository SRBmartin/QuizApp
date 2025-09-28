using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Tags;

namespace QuizApp.Application.Features.Quizes.UpdateTags;

public record UpdateQuizTagsCommand
(
    Guid QuizId,
    List<Guid> TagIds
): IRequest<Result<IReadOnlyList<TagDto>>>;