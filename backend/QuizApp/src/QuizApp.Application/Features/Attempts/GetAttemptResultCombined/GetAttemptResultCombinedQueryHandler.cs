using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Application.Features.Attempts.GetAttemptResultReview;
using QuizApp.Application.Features.Attempts.GetAttemptResultSummary;

namespace QuizApp.Application.Features.Attempts.GetAttemptResultCombined;

public class GetAttemptResultCombinedQueryHandler (
    IMediator mediator
) : IRequestHandler<GetAttemptResultCombinedQuery, Result<AttemptResultCombinedDto>>
{
    public async Task<Result<AttemptResultCombinedDto>> Handle(GetAttemptResultCombinedQuery query, CancellationToken cancellationToken)
    {
        var s = await mediator.Send(new GetAttemptResultSummaryQuery(query.AttemptId), cancellationToken);
        if (!s.IsSuccess) return Result<AttemptResultCombinedDto>.Failure(s.Error);

        var r = await mediator.Send(new GetAttemptResultReviewQuery(query.AttemptId), cancellationToken);
        if (!r.IsSuccess) return Result<AttemptResultCombinedDto>.Failure(r.Error);

        return Result<AttemptResultCombinedDto>.Success(new AttemptResultCombinedDto
        {
            Summary = s.Value!,
            Review = r.Value!
        });
    }
}