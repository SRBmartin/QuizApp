using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Abstractions.Storage;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Application.DTOs.User;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Quizes.GetLeaderboard;

public class GetQuizLeaderboardQueryHandler(
    IAttemptRepository attemptRepository,
    IQuizRepository quizRepository,
    IImageStorage imageStorage,
    ICurrentUser currentUser
) : IRequestHandler<GetQuizLeaderboardQuery, Result<LeaderboardDto>>
{
    private sealed class PerUserBestRow
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public string? Photo { get; set; }
        public int BestTotal { get; set; }
    }

    private sealed class BestWithTimeRow
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public string? Photo { get; set; }
        public int BestTotal { get; set; }
        public DateTimeOffset? BestSubmittedAt { get; set; }
    }

    public async Task<Result<LeaderboardDto>> Handle(GetQuizLeaderboardQuery query, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.FindByIdAsync(query.QuizId, cancellationToken);
        if (quiz is null)
            return Result<LeaderboardDto>.Failure(new Error("quiz.not_found", "Quiz not found."));

        var maxScore = quiz.Questions.Sum(q => q.Points);

        var baseQ = attemptRepository
            .Query()
            .Where(a => a.QuizId == query.QuizId
                        && a.Status == AttemptStatus.Completed
                        && a.SubmittedAt != null);

        var now = DateTimeOffset.UtcNow;
        if (!string.IsNullOrWhiteSpace(query.Period))
        {
            if (query.Period!.Equals("month", StringComparison.OrdinalIgnoreCase))
            {
                var from = now.AddDays(-30);
                baseQ = baseQ.Where(a => a.SubmittedAt >= from);
            }
            else if (query.Period!.Equals("week", StringComparison.OrdinalIgnoreCase))
            {
                var from = now.AddDays(-7);
                baseQ = baseQ.Where(a => a.SubmittedAt >= from);
            }
        }

        var perUserBest =
            baseQ
            .GroupBy(a => new { a.UserId, a.User.Username, a.User.Photo })
            .Select(g => new PerUserBestRow
            {
                UserId = g.Key.UserId,
                Username = g.Key.Username,
                Photo = g.Key.Photo,
                BestTotal = g.Max(x => x.TotalScore)
            });

        var totalParticipants = perUserBest.Count();

        var bestWithTimeQ =
            baseQ
            .Join(perUserBest,
                a => new { a.UserId, Score = a.TotalScore },
                b => new { b.UserId, Score = b.BestTotal },
                (a, b) => new { b.UserId, b.Username, b.Photo, b.BestTotal, a.SubmittedAt })
            .GroupBy(x => new { x.UserId, x.Username, x.Photo, x.BestTotal })
            .Select(g => new BestWithTimeRow
            {
                UserId = g.Key.UserId,
                Username = g.Key.Username,
                Photo = g.Key.Photo,
                BestTotal = g.Key.BestTotal,
                BestSubmittedAt = g.Min(x => x.SubmittedAt)
            });

        var topRows = bestWithTimeQ
            .OrderByDescending(x => x.BestTotal)
            .ThenBy(x => x.BestSubmittedAt)
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
                TotalScore = x.BestTotal,
                Percentage = maxScore > 0 ? (int)Math.Round(x.BestTotal * 100d / maxScore) : 0,
                SubmittedAt = x.BestSubmittedAt
            })
            .ToList();

        LeaderboardEntryDto? myEntry = null;
        if (currentUser.IsAuthenticated && currentUser.Id is not null)
        {
            var me = bestWithTimeQ.FirstOrDefault(x => x.UserId == currentUser.Id.Value);
            if (me is not null)
            {
                var betterCount = bestWithTimeQ.Count(x =>
                    x.BestTotal > me.BestTotal ||
                    (x.BestTotal == me.BestTotal && x.BestSubmittedAt < me.BestSubmittedAt));

                myEntry = new LeaderboardEntryDto
                {
                    Rank = betterCount + 1,
                    User = new UserPublicDto { Id = me.UserId, Username = me.Username, Photo = me.Photo },
                    TotalScore = me.BestTotal,
                    Percentage = maxScore > 0 ? (int)Math.Round(me.BestTotal * 100d / maxScore) : 0,
                    SubmittedAt = me.BestSubmittedAt
                };
            }
        }

        var dto = new LeaderboardDto
        {
            QuizId = quiz.Id,
            QuizName = quiz.Name,
            Period = string.IsNullOrWhiteSpace(query.Period) ? "all" : query.Period!.ToLowerInvariant(),
            MaxScore = maxScore,
            TotalParticipants = totalParticipants,
            Top = top,
            MyEntry = myEntry
        };

        return Result<LeaderboardDto>.Success(dto);
    }

    #region Helpers

    private static string? BuildPublicPhotoUrl(string? stored, IImageStorage storage)
    {
        if (string.IsNullOrWhiteSpace(stored)) return null;

        if (Uri.TryCreate(stored, UriKind.Absolute, out var absolute))
            return absolute.ToString();

        var slash = stored.IndexOf('/');
        if (slash > 0 && slash < stored.Length - 1)
        {
            var bucket = stored.Substring(0, slash);
            var objectKey = stored.Substring(slash + 1);
            var url = storage.TryBuildPublicUrl(bucket, objectKey);
            if (url is not null) return url.ToString();
        }

        return stored;
    }

    #endregion

}