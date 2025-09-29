using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Attempts.GetAttemptResultReview;

public class GetAttemptResultReviewQueryHandler(
    IAttemptRepository attemptRepository,
    IQuizRepository quizRepository,
    ICurrentUser currentUser
) : IRequestHandler<GetAttemptResultReviewQuery, Result<AttemptResultReviewDto>>
{
    public async Task<Result<AttemptResultReviewDto>> Handle(GetAttemptResultReviewQuery query, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<AttemptResultReviewDto>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var attempt = await attemptRepository.FindByIdAsync(query.AttemptId, cancellationToken);

        var isOwner = attempt.UserId == currentUser.Id.Value;
        var isAdmin = currentUser.Role!.Equals("Admin");

        if (attempt is null || (!isOwner && !isAdmin))
            return Result<AttemptResultReviewDto>.Failure(new Error("attempt.not_found", "Attempt not found."));

        if (!isOwner && !isAdmin)
            return Result<AttemptResultReviewDto>.Failure(new Error("auth.forbidden", "Forbidden."));

        var quiz = await quizRepository.FindByIdAsync(attempt.QuizId, cancellationToken);
        if (quiz is null)
            return Result<AttemptResultReviewDto>.Failure(new Error("quiz.not_found", "Quiz not found."));

        var items = new List<AttemptReviewItemDto>();
        var itemsByQ = attempt.Items.ToDictionary(i => i.QuestionId, i => i);

        foreach (var q in quiz.Questions.OrderBy(x => x.CreatedAt))
        {
            itemsByQ.TryGetValue(q.Id, out var item);

            var dto = new AttemptReviewItemDto
            {
                QuestionId = q.Id,
                Question = q.Question,
                Type = q.Type,
                Points = q.Points,
                AwardedScore = item?.AwardedScore ?? 0,
                IsCorrect = item?.IsCorrect ?? false
            };

            if (q.Type == QuestionType.FillIn)
            {
                dto.TextAnswer = new AttemptReviewTextDto
                {
                    SubmittedText = item?.TextAnswer?.SubmittedText,
                    ExpectedText = q.TextAnswer?.Text
                };
            }
            else
            {
                dto.Options = q.Choices.Select(c => new AttemptReviewOptionDto
                {
                    Id = c.Id,
                    Text = c.Label,
                    IsCorrect = c.IsCorrect,
                    SelectedByUser = item?.SelectedChoices.Any(sc => sc.ChoiceId == c.Id) ?? false
                }).ToList();
            }

            items.Add(dto);
        }

        var result = new AttemptResultReviewDto
        {
            AttemptId = attempt.Id,
            Items = items
        };

        return Result<AttemptResultReviewDto>.Success(result);
    }
}