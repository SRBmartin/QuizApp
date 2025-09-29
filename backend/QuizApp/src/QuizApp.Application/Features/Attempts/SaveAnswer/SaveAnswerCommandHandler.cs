using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Mappings;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;
using QuizApp.Domain.Repositories.Writer;

namespace QuizApp.Application.Features.Attempts.SaveAnswer;

public class SaveAnswerCommandHandler(
    IAttemptRepository attemptRepository,
    IQuizRepository quizRepository,
    IAttemptAnswerWriter answerWriter,
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

        var now = DateTimeOffset.UtcNow;
        var elapsed = (int)(now - attempt.StartedAt).TotalSeconds;
        if (elapsed >= quiz.TimeInSeconds)
        {
            var total = attempt.Items.Sum(i => i.AwardedScore);
            attempt.Complete(now, total);
            await uow.SaveChangesAsync(cancellationToken);
            return Result<AttemptQuestionDto>.Failure(new Error("attempt.time_expired", "Time expired."));
        }

        var question = quiz.Questions.FirstOrDefault(q => q.Id == command.QuestionId);
        if (question is null)
            return Result<AttemptQuestionDto>.Failure(new Error("question.not_found", "Question not found."));

        await answerWriter.UpsertAsync(
            attemptId: attempt.Id,
            questionId: question.Id,
            selectedChoiceIds: command.SelectedChoiceIds,
            submittedText: command.SubmittedText,
            answeredAtUtc: now,
            cancellationToken: cancellationToken);

        var freshAttempt = await attemptRepository.FindByIdAsync(attempt.Id, cancellationToken);
        var answeredIds = (freshAttempt ?? attempt).Items.Select(i => i.QuestionId).ToHashSet();

        var next = quiz.Questions.OrderBy(q => q.CreatedAt).FirstOrDefault(q => !answeredIds.Contains(q.Id));

        if (next is null)
        {
            return Result<AttemptQuestionDto>.Success(new AttemptQuestionDto
            {
                Id = question.Id,
                QuizId = quiz.Id,
                Question = question.Question,
                Type = question.Type.ToDto(),
                Points = question.Points,
                Options = question.Choices.Select(c => new AttemptQuestionOptionDto { Id = c.Id, Text = c.Label }).ToList(),
                IsLast = true
            });
        }

        return Result<AttemptQuestionDto>.Success(new AttemptQuestionDto
        {
            Id = next.Id,
            QuizId = next.QuizId,
            Question = next.Question,
            Type = next.Type.ToDto(),
            Points = next.Points,
            Options = next.Choices.Select(c => new AttemptQuestionOptionDto { Id = c.Id, Text = c.Label }).ToList(),
            IsLast = quiz.Questions.Count == answeredIds.Count + 1
        });
    }
}
