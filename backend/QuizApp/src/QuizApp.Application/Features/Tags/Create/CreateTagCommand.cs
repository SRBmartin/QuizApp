using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Tags;

namespace QuizApp.Application.Features.Tags.Create;

public record CreateTagCommand
(
    string Name
) : IRequest<Result<TagDto>>;