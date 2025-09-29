using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;

namespace QuizApp.Application.Features.Attempts.GetAttemptResultReview;

public record GetAttemptResultReviewQuery
(
    Guid AttemptId
) : IRequest<Result<AttemptResultReviewDto>>;