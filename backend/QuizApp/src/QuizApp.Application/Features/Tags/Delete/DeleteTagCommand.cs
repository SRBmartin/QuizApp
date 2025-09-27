using MediatR;
using QuizApp.Application.Common.Result;

namespace QuizApp.Application.Features.Tags.Delete;

public record DeleteTagCommand
(
    Guid Id
) : IRequest<Result<Unit>>;