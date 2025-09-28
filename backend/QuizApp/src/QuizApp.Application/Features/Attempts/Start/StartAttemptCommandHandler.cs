using AutoMapper;
using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Mappings;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Attempts.Start;

class StartAttemptCommandHandler (
    IAttemptRepository attemptRepository,
    IQuizRepository quizRepository,
    IUnitOfWork uow,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<StartAttemptCommand, Result<AttemptQuestionDto>>
{
    public async Task<Result<AttemptQuestionDto>> Handle(StartAttemptCommand command, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<AttemptQuestionDto>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var quiz = await quizRepository.FindByIdAsync(command.QuizId, cancellationToken);
        if (quiz is null || !quiz.IsPublished || quiz.IsDeleted || quiz.Questions.Count == 0)
            return Result<AttemptQuestionDto>.Failure(new Error("quiz.invalid", "Quiz not available."));

        var existing = await attemptRepository.FindActiveForUserQuizAsync(currentUser.Id.Value, quiz.Id, cancellationToken);
        if (existing is not null)
        {
            return await BuildNextQuestion(existing, quiz, cancellationToken);
        }

        var now = DateTimeOffset.UtcNow;
        var attempt = Attempt.Start(Guid.NewGuid(), quiz.Id, currentUser.Id.Value, now);

        await attemptRepository.AddAsync(attempt, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return await BuildNextQuestion(attempt, quiz, cancellationToken);
    }

    #region Helpers

    private async Task<Result<AttemptQuestionDto>> BuildNextQuestion(Attempt attempt, Quiz quiz, CancellationToken cancellationToken)
    {
        var answeredQIds = attempt.Items.Select(i => i.QuestionId).ToHashSet();
        var next = quiz.Questions
            .OrderBy(q => q.CreatedAt)
            .FirstOrDefault(q => !answeredQIds.Contains(q.Id));

        if (next is null)
            return Result<AttemptQuestionDto>.Failure(new Error("attempt.completed", "All questions answered."));

        var dto = new AttemptQuestionDto
        {
            Id = next.Id,
            QuizId = quiz.Id,
            Question = next.Question,
            Type = next.Type.ToDto(),
            Points = next.Points,
            Options = next.Choices
                .Select(c => new AttemptQuestionOptionDto { Id = c.Id, Text = c.Label })
                .ToList(),
            IsLast = quiz.Questions.Count == answeredQIds.Count + 1
        };

        return Result<AttemptQuestionDto>.Success(dto);
    }

    #endregion

}
