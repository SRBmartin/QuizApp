using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;

namespace QuizApp.Application.Features.Attempts.GetAttemptResultCombined;

public record GetAttemptResultCombinedQuery
(
    Guid AttemptId
) : IRequest<Result<AttemptResultCombinedDto>>;