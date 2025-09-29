using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Abstractions.Storage;
using QuizApp.Application.Common.Page;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Application.DTOs.User;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Attempts.GetQuizAttempts;

public class GetQuizAttemptsQueryHandler(
    IAttemptRepository attemptRepository,
    IImageStorage imageStorage,
    ICurrentUser currentUser
) : IRequestHandler<GetQuizAttemptsQuery, Result<PagedListDto<QuizAttemptListItemDto>>>
{
    private sealed class Row
    {
        public Guid AttemptId { get; set; }
        public Guid QuizId { get; set; }
        public string QuizName { get; set; } = default!;
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public string? Photo { get; set; }
        public Domain.Enums.AttemptStatus Status { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public int TotalScore { get; set; }
        public int MaxScore { get; set; }
        public int Percentage { get; set; }
    }

    public Task<Result<PagedListDto<QuizAttemptListItemDto>>> Handle(GetQuizAttemptsQuery query, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Task.FromResult(Result<PagedListDto<QuizAttemptListItemDto>>.Failure(
                new Error("auth.unauthorized", "Unauthorized.")));

        var userId = currentUser.Id.Value;

        var baseQ = attemptRepository
            .Query()
            .Where(a => a.QuizId == query.QuizId);

        bool isAdmin = currentUser.Role!.Equals("Admin");
        if (!isAdmin)
        {
            baseQ = baseQ.Where(a => a.UserId == userId);
        }
        else if (query.UserId.HasValue)
        {
            baseQ = baseQ.Where(a => a.UserId == query.UserId.Value);
        }

        if (query.StatusFilter.HasValue)
            baseQ = baseQ.Where(a => a.Status == query.StatusFilter.Value);

        var total = baseQ.Count();

        var q = baseQ
            .OrderByDescending(a => a.SubmittedAt ?? a.StartedAt)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(a => new Row
            {
                AttemptId = a.Id,
                QuizId = a.QuizId,
                QuizName = a.Quiz.Name,

                UserId = a.UserId,
                Username = a.User.Username,
                Photo = a.User.Photo,

                Status = a.Status,
                StartedAt = a.StartedAt,
                SubmittedAt = a.SubmittedAt,

                TotalScore = (a.Items.Sum(i => (int?)i.AwardedScore) ?? 0),
                MaxScore = (a.Quiz.Questions.Sum(q2 => (int?)q2.Points) ?? 0),

                Percentage = ((a.Quiz.Questions.Sum(q2 => (int?)q2.Points) ?? 0) > 0)
                    ? (int)Math.Round(((a.Items.Sum(i => (int?)i.AwardedScore) ?? 0) * 100d) /
                                      (a.Quiz.Questions.Sum(q2 => (int?)q2.Points) ?? 0))
                    : 0
            });

        var rows = q.ToList();

        string? ToPublic(string? stored)
        {
            if (string.IsNullOrWhiteSpace(stored)) return null;
            if (Uri.TryCreate(stored, UriKind.Absolute, out var abs)) return abs.ToString();
            var slash = stored!.IndexOf('/');
            if (slash > 0 && slash < stored.Length - 1)
            {
                var bucket = stored[..slash];
                var key = stored[(slash + 1)..];
                var url = imageStorage.TryBuildPublicUrl(bucket, key);
                if (url is not null) return url.ToString();
            }
            return stored;
        }

        var items = rows.Select(r => new QuizAttemptListItemDto
        {
            AttemptId = r.AttemptId,
            QuizId = r.QuizId,
            QuizName = r.QuizName,

            User = new UserPublicDto { Id = r.UserId, Username = r.Username, Photo = ToPublic(r.Photo) },

            Status = r.Status,
            StartedAt = r.StartedAt,
            SubmittedAt = r.SubmittedAt,

            TotalScore = r.TotalScore,
            MaxScore = r.MaxScore,
            Percentage = r.Percentage,

            DurationSeconds = (r.SubmittedAt.HasValue)
                ? (int?)Math.Max(0, (r.SubmittedAt.Value - r.StartedAt).TotalSeconds)
                : null
        }).ToList();

        var result = new PagedListDto<QuizAttemptListItemDto>(items, total, query.Skip, query.Take);
        return Task.FromResult(Result<PagedListDto<QuizAttemptListItemDto>>.Success(result));
    }
}