using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;

namespace QuizApp.Application.Features.Attempts.GetAttemptState;

public record GetAttemptStateQuery
(
    Guid AttemptId
) : IRequest<Result<AttemptDto>>;