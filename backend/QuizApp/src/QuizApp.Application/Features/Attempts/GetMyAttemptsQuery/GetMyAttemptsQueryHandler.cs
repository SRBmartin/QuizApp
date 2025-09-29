using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Page;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Attempts.GetMyAttemptsQuery;

public class GetMyAttemptsQueryHandler(
    IAttemptRepository attemptRepository,
    ICurrentUser currentUser
) : IRequestHandler<GetMyAttemptsQuery, Result<PagedListDto<MyAttemptListItemDto>>>
{
    public async Task<Result<PagedListDto<MyAttemptListItemDto>>> Handle(GetMyAttemptsQuery query, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<PagedListDto<MyAttemptListItemDto>>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var userId = currentUser.Id.Value;

        var baseQ = attemptRepository
            .Query()
            .Where(a => a.UserId == userId);

        if (query.StatusFilter.HasValue)
            baseQ = baseQ.Where(a => a.Status == query.StatusFilter.Value);

        if (query.QuizId.HasValue)
            baseQ = baseQ.Where(a => a.QuizId == query.QuizId.Value);

        var total = baseQ.Count();

        var q = baseQ
            .OrderByDescending(a => a.SubmittedAt ?? a.StartedAt)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(a => new MyAttemptListItemDto
            {
                AttemptId = a.Id,
                QuizId = a.QuizId,
                QuizName = a.Quiz.Name,

                Status = a.Status,
                StartedAt = a.StartedAt,
                SubmittedAt = a.SubmittedAt,

                TotalScore = (a.Items.Sum(i => (int?)i.AwardedScore) ?? 0),
                MaxScore = (a.Quiz.Questions.Sum(q2 => (int?)q2.Points) ?? 0),

                Percentage = (a.Quiz.Questions.Sum(q2 => (int?)q2.Points) ?? 0) > 0
                    ? (int)Math.Round(((a.Items.Sum(i => (int?)i.AwardedScore) ?? 0) * 100d) /
                                      (a.Quiz.Questions.Sum(q2 => (int?)q2.Points) ?? 0))
                    : 0,

                DurationSeconds = null
            });

        var items = q.ToList();

        var result = new PagedListDto<MyAttemptListItemDto>(items, total, query.Skip, query.Take);
        return Result<PagedListDto<MyAttemptListItemDto>>.Success(result);
    }
}