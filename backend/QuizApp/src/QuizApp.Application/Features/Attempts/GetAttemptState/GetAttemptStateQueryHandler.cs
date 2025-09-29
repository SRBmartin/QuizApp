using AutoMapper;
using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Attempts.GetAttemptState;

public class GetAttemptStateQueryHandler (
    IAttemptRepository attemptRepository,
    IQuizRepository quizRepository,
    IMapper mapper
) : IRequestHandler<GetAttemptStateQuery, Result<AttemptDto>>
{
    public async Task<Result<AttemptDto>> Handle(GetAttemptStateQuery query, CancellationToken cancellationToken)
    {
        var attempt = await attemptRepository.FindByIdAsync(query.AttemptId, cancellationToken);
        if (attempt is null)
            return Result<AttemptDto>.Failure(new Error("attempt.not_found", "Attempt not found."));

        var quiz = await quizRepository.FindByIdAsync(attempt.QuizId, cancellationToken);
        if (quiz is null)
            return Result<AttemptDto>.Failure(new Error("quiz.not_found", "Quiz not found."));

        var answered = attempt.Items.Count;
        var total = quiz.Questions.Count;

        var elapsed = (int)(DateTimeOffset.UtcNow - attempt.StartedAt).TotalSeconds;
        var timeLeft = Math.Max(0, quiz.TimeInSeconds - elapsed);

        var dto = mapper.Map<AttemptDto>(attempt);
        dto.AnsweredCount = answered;
        dto.TotalQuestions = total;
        dto.TimeLeftSeconds = timeLeft;

        return Result<AttemptDto>.Success(dto);
    }

}
