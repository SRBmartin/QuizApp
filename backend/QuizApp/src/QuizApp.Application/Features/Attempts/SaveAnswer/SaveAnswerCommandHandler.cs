using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Mappings;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Attempts.SaveAnswer;

public class SaveAnswerCommandHandler (
    IAttemptRepository attemptRepository,
    IQuizRepository quizRepository,
    IUnitOfWork uow,
    ICurrentUser currentUser
) : IRequestHandler<SaveAnswerCommand, Result<AttemptQuestionDto>>
{
    public async Task<Result<AttemptQuestionDto>> Handle(SaveAnswerCommand command, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<AttemptQuestionDto>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var attempt = await attemptRepository.FindByIdAsync(command.AttemptId, cancellationToken);
        if (attempt is null || attempt.UserId != currentUser.Id.Value)
            return Result<AttemptQuestionDto>.Failure(new Error("attempt.not_found", "Attempt not found."));

        if (attempt.Status != AttemptStatus.InProgress)
            return Result<AttemptQuestionDto>.Failure(new Error("attempt.completed", "Attempt already completed."));

        var quiz = await quizRepository.FindByIdAsync(attempt.QuizId, cancellationToken);
        if (quiz is null)
            return Result<AttemptQuestionDto>.Failure(new Error("quiz.not_found", "Quiz not found."));

        var timeLimit = quiz.TimeInSeconds;
        var now = DateTimeOffset.UtcNow;
        var elapsed = (int)(now - attempt.StartedAt).TotalSeconds;
        if (elapsed >= timeLimit)
        {
            var total = attempt.Items.Sum(i => i.AwardedScore);
            attempt.Complete(now, total);
            await uow.SaveChangesAsync(cancellationToken);
            return Result<AttemptQuestionDto>.Failure(new Error("attempt.time_expired", "Time expired."));
        }

        var question = quiz.Questions.FirstOrDefault(q => q.Id == command.QuestionId);
        if (question is null)
            return Result<AttemptQuestionDto>.Failure(new Error("question.not_found", "Question not found."));

        var existing = attempt.Items.FirstOrDefault(i => i.QuestionId == question.Id);

        if (existing is null)
        {
            existing = AttemptItem.For(Guid.NewGuid(), attempt.Id, question.Id, now, 0, false);
            attempt.Items.Add(existing);
        }
        else
        {
            existing.AnsweredAt = now;
            existing.SelectedChoices.Clear();
            existing.TextAnswer = null;
            existing.AwardedScore = 0;
            existing.IsCorrect = false;
        }

        var awarded = 0;
        var correct = false;

        if (question.Type == QuestionType.Single || question.Type == QuestionType.TrueFalse)
        {
            if (command.SelectedChoiceIds is null || command.SelectedChoiceIds.Count != 1)
                return Result<AttemptQuestionDto>.Failure(new Error("answer.invalid", "Single choice required."));

            var chosenId = command.SelectedChoiceIds[0];
            var chosen = question.Choices.FirstOrDefault(c => c.Id == chosenId);
            if (chosen is null)
                return Result<AttemptQuestionDto>.Failure(new Error("answer.invalid_choice", "Choice invalid."));

            existing.SelectedChoices.Add(new AttemptItemChoice { AttemptItemId = existing.Id, ChoiceId = chosen.Id });

            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            correct = correctChoice is not null && correctChoice.Id == chosen.Id;
            awarded = correct ? question.Points : 0;
        }
        else if (question.Type == QuestionType.Multi)
        {
            if (command.SelectedChoiceIds is null || command.SelectedChoiceIds.Count == 0)
                return Result<AttemptQuestionDto>.Failure(new Error("answer.invalid", "At least one choice required."));

            var set = command.SelectedChoiceIds.ToHashSet();
            var allExist = question.Choices.Where(c => set.Contains(c.Id)).Count() == set.Count;
            if (!allExist)
                return Result<AttemptQuestionDto>.Failure(new Error("answer.invalid_choice", "Choice invalid."));

            foreach (var cid in set)
                existing.SelectedChoices.Add(new AttemptItemChoice { AttemptItemId = existing.Id, ChoiceId = cid });

            var correctSet = question.Choices.Where(c => c.IsCorrect).Select(c => c.Id).OrderBy(x => x).ToList();
            var chosenSet = set.OrderBy(x => x).ToList();

            correct = correctSet.SequenceEqual(chosenSet);
            awarded = correct ? question.Points : 0;
        }
        else if (question.Type == QuestionType.FillIn)
        {
            if (string.IsNullOrWhiteSpace(command.SubmittedText))
                return Result<AttemptQuestionDto>.Failure(new Error("answer.invalid", "Text answer required."));

            existing.TextAnswer = new AttemptItemText
            {
                AttemptItemId = existing.Id,
                SubmittedText = command.SubmittedText.Trim()
            };

            var expected = question.TextAnswer?.Text?.Trim();
            if (!string.IsNullOrEmpty(expected))
            {
                correct = string.Equals(
                    expected,
                    existing.TextAnswer.SubmittedText,
                    StringComparison.OrdinalIgnoreCase);
            }
            awarded = correct ? question.Points : 0;
        }
        else
        {
            return Result<AttemptQuestionDto>.Failure(new Error("question.type_not_supported", "Unsupported question type."));
        }

        existing.AwardedScore = awarded;
        existing.IsCorrect = correct;

        await uow.SaveChangesAsync(cancellationToken);

        var answeredIds = attempt.Items.Select(i => i.QuestionId).ToHashSet();
        var next = quiz.Questions.OrderBy(q => q.CreatedAt).FirstOrDefault(q => !answeredIds.Contains(q.Id));

        if (next is null)
            return Result<AttemptQuestionDto>.Success(new AttemptQuestionDto
            {
                Id = question.Id,
                QuizId = quiz.Id,
                Question = question.Question,
                Type = question.Type.ToDto(),
                Points = question.Points,
                Options = question.Choices
                    .Select(c => new AttemptQuestionOptionDto { Id = c.Id, Text = c.Label })
                    .ToList(),
                IsLast = true
            });

        return Result<AttemptQuestionDto>.Success(new AttemptQuestionDto
        {
            Id = next.Id,
            QuizId = quiz.Id,
            Question = next.Question,
            Type = next.Type.ToDto(),
            Points = next.Points,
            Options = next.Choices
                .Select(c => new AttemptQuestionOptionDto { Id = c.Id, Text = c.Label })
                .ToList(),
            IsLast = quiz.Questions.Count == answeredIds.Count + 1
        });

    }

}
