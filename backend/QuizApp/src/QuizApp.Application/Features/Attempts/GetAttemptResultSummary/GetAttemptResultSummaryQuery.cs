using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;

namespace QuizApp.Application.Features.Attempts.GetAttemptResultSummary;

public record GetAttemptResultSummaryQuery
(
    Guid AttemptId    
) : IRequest<Result<AttemptResultSummaryDto>>;