using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Tags;

namespace QuizApp.Application.Features.Tags.Update;

public record UpdateTagCommand
(
    Guid Id,
    string Name
) : IRequest<Result<TagDto>>;