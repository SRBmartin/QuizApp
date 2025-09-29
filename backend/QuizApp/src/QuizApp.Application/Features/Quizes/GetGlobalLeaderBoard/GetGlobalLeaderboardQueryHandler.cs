using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Abstractions.Storage;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Application.DTOs.User;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Quizes.GetGlobalLeaderBoard;

public class GetGlobalLeaderboardQueryHandler(
    IAttemptRepository attemptRepository,
    IQuizRepository quizRepository,
    IImageStorage imageStorage,
    ICurrentUser currentUser
) : IRequestHandler<GetGlobalLeaderboardQuery, Result<GlobalLeaderboardDto>>
{
    private sealed class PerUserQuizBest
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public string? Photo { get; set; }
        public Guid QuizId { get; set; }
        public int BestTotal { get; set; }
        public DateTimeOffset? BestSubmittedAt { get; set; }
    }

    private sealed class PerUserTotals
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public string? Photo { get; set; }
        public int TotalSum { get; set; }
        public DateTimeOffset? FirstSubmittedAt { get; set; }
    }

    public Task<Result<GlobalLeaderboardDto>> Handle(GetGlobalLeaderboardQuery query, CancellationToken cancellationToken)
    {
        var baseQ = attemptRepository
            .Query()
            .Where(a => a.Status == AttemptStatus.Completed &&
                        a.SubmittedAt != null &&
                        a.Quiz.IsDeleted == false);

        var now = DateTimeOffset.UtcNow;
        if (!string.IsNullOrWhiteSpace(query.Period))
        {
            if (query.Period.Equals("month", StringComparison.OrdinalIgnoreCase))
                baseQ = baseQ.Where(a => a.SubmittedAt >= now.AddDays(-30));
            else if (query.Period.Equals("week", StringComparison.OrdinalIgnoreCase))
                baseQ = baseQ.Where(a => a.SubmittedAt >= now.AddDays(-7));
        }

        var perUserQuizBestScore =
            baseQ
            .GroupBy(a => new { a.UserId, a.User.Username, a.User.Photo, a.QuizId })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.Username,
                g.Key.Photo,
                g.Key.QuizId,
                BestTotal = g.Max(x => x.TotalScore)
            });

        var perUserQuizBest =
            baseQ
            .Join(perUserQuizBestScore,
                  a => new { a.UserId, a.QuizId, Score = a.TotalScore },
                  b => new { b.UserId, b.QuizId, Score = b.BestTotal },
                  (a, b) => new { b.UserId, b.Username, b.Photo, b.QuizId, b.BestTotal, a.SubmittedAt })
            .GroupBy(x => new { x.UserId, x.Username, x.Photo, x.QuizId, x.BestTotal })
            .Select(g => new PerUserQuizBest
            {
                UserId = g.Key.UserId,
                Username = g.Key.Username,
                Photo = g.Key.Photo,
                QuizId = g.Key.QuizId,
                BestTotal = g.Key.BestTotal,
                BestSubmittedAt = g.Min(x => x.SubmittedAt)
            });

        var perUserTotals =
            perUserQuizBest
            .GroupBy(x => new { x.UserId, x.Username, x.Photo })
            .Select(g => new PerUserTotals
            {
                UserId = g.Key.UserId,
                Username = g.Key.Username,
                Photo = g.Key.Photo,
                TotalSum = g.Sum(x => x.BestTotal),
                FirstSubmittedAt = g.Min(x => x.BestSubmittedAt)
            });

        var totalParticipants = perUserTotals.Count();

        var maxScoreTotal = quizRepository
            .Query()
            .Where(q => q.IsDeleted == false)
            .Select(q => (q.Questions.Sum(qq => (int?)qq.Points) ?? 0))
            .Sum();

        var topRows = perUserTotals
            .OrderByDescending(x => x.TotalSum)
            .ThenBy(x => x.FirstSubmittedAt)
            .Take(query.Take)
            .ToList();

        var top = topRows
            .Select((x, i) => new LeaderboardEntryDto
            {
                Rank = i + 1,
                User = new UserPublicDto
                {
                    Id = x.UserId,
                    Username = x.Username,
                    Photo = BuildPublicPhotoUrl(x.Photo, imageStorage)
                },
                TotalScore = x.TotalSum,
                Percentage = maxScoreTotal > 0 ? (int)Math.Round(x.TotalSum * 100d / maxScoreTotal) : 0,
                SubmittedAt = x.FirstSubmittedAt
            })
            .ToList();

        LeaderboardEntryDto? myEntry = null;
        if (currentUser.IsAuthenticated && currentUser.Id is not null)
        {
            var me = perUserTotals.FirstOrDefault(x => x.UserId == currentUser.Id.Value);
            if (me is not null)
            {
                var better = perUserTotals.Count(x =>
                    x.TotalSum > me.TotalSum ||
                    (x.TotalSum == me.TotalSum && x.FirstSubmittedAt < me.FirstSubmittedAt));

                myEntry = new LeaderboardEntryDto
                {
                    Rank = better + 1,
                    User = new UserPublicDto
                    {
                        Id = me.UserId,
                        Username = me.Username,
                        Photo = BuildPublicPhotoUrl(me.Photo, imageStorage)
                    },
                    TotalScore = me.TotalSum,
                    Percentage = maxScoreTotal > 0 ? (int)Math.Round(me.TotalSum * 100d / maxScoreTotal) : 0,
                    SubmittedAt = me.FirstSubmittedAt
                };
            }
        }

        var dto = new GlobalLeaderboardDto
        {
            Period = string.IsNullOrWhiteSpace(query.Period) ? "all" : query.Period!.ToLowerInvariant(),
            MaxScoreTotal = maxScoreTotal,
            TotalParticipants = totalParticipants,
            Top = top,
            MyEntry = myEntry
        };

        return Task.FromResult(Result<GlobalLeaderboardDto>.Success(dto));
    }

    private static string? BuildPublicPhotoUrl(string? stored, IImageStorage storage)
    {
        if (string.IsNullOrWhiteSpace(stored)) return null;

        if (Uri.TryCreate(stored, UriKind.Absolute, out var abs))
            return abs.ToString();

        var slash = stored.IndexOf('/');
        if (slash > 0 && slash < stored.Length - 1)
        {
            var bucket = stored[..slash];
            var objectKey = stored[(slash + 1)..];
            var url = storage.TryBuildPublicUrl(bucket, objectKey);
            if (url is not null) return url.ToString();
        }

        return stored;
    }
}