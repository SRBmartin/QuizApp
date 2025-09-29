using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Repositories;
using System.Threading;

namespace QuizApp.Application.Features.Attempts.GetAttemptResultSummary;

public class GetAttemptResultSummaryQueryHandler(
    IAttemptRepository attemptRepository,
    IQuizRepository quizRepository,
    ICurrentUser currentUser
) : IRequestHandler<GetAttemptResultSummaryQuery, Result<AttemptResultSummaryDto>>
{
    public async Task<Result<AttemptResultSummaryDto>> Handle(GetAttemptResultSummaryQuery query, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<AttemptResultSummaryDto>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var attempt = await attemptRepository.FindByIdAsync(query.AttemptId, cancellationToken);

        var isOwner = attempt.UserId == currentUser.Id.Value;
        var isAdmin = currentUser.Role!.Equals("Admin");

        if (attempt is null || (!isOwner && !isAdmin))
            return Result<AttemptResultSummaryDto>.Failure(new Error("attempt.not_found", "Attempt not found."));

        if (!isOwner && !isAdmin)
            return Result<AttemptResultSummaryDto>.Failure(new Error("auth.forbidden", "Forbidden."));

        var quiz = await quizRepository.FindByIdAsync(attempt.QuizId, cancellationToken);
        if (quiz is null)
            return Result<AttemptResultSummaryDto>.Failure(new Error("quiz.not_found", "Quiz not found."));

        var totalScore = attempt.Items.Sum(i => i.AwardedScore);
        var correctAnswers = attempt.Items.Count(i => i.IsCorrect);

        var questions = quiz.Questions ?? new List<QuizApp.Domain.Entities.QuizQuestion>();
        var maxScore = questions.Sum(q => q.Points);
        var totalQuestions = questions.Count;

        var percentage = maxScore > 0 ? (int)Math.Round((double)totalScore * 100d / maxScore) : 0;

        int? durationSeconds = null;
        if (attempt.SubmittedAt.HasValue)
            durationSeconds = (int)Math.Max(0, (attempt.SubmittedAt.Value - attempt.StartedAt).TotalSeconds);

        var dto = new AttemptResultSummaryDto
        {
            AttemptId = attempt.Id,
            QuizId = quiz.Id,
            QuizName = quiz.Name,
            Status = attempt.Status,

            TotalQuestions = totalQuestions,
            CorrectAnswers = correctAnswers,

            TotalScore = totalScore,
            MaxScore = maxScore,
            Percentage = percentage,

            TimeLimitSeconds = quiz.TimeInSeconds,
            DurationSeconds = durationSeconds,

            StartedAt = attempt.StartedAt,
            SubmittedAt = attempt.SubmittedAt
        };

        return Result<AttemptResultSummaryDto>.Success(dto);
    }
}