using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Tags;

namespace QuizApp.Application.Features.Tags.List;

public record ListTagsQuery
(
    int? Skip = null,
    int? Take = null
) : IRequest<Result<IReadOnlyList<TagDto>>>;