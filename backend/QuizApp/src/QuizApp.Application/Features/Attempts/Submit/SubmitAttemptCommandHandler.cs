using AutoMapper;
using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Attempts.Submit;

public class SubmitAttemptCommandHandler (
    IAttemptRepository attemptRepository,
    IQuizRepository quizRepository,
    IUnitOfWork uow,
    IMapper mapper
) : IRequestHandler<SubmitAttemptCommand, Result<AttemptDto>>
{
    public async Task<Result<AttemptDto>> Handle(SubmitAttemptCommand command, CancellationToken cancellationToken)
    {
        var attempt = await attemptRepository.FindByIdAsync(command.AttemptId, cancellationToken);
        if (attempt is null)
            return Result<AttemptDto>.Failure(new Error("attempt.not_found", "Attempt not found."));

        if (attempt.Status == AttemptStatus.Completed)
        {
            var dtoC = mapper.Map<AttemptDto>(attempt);
            dtoC.AnsweredCount = attempt.Items.Count;
            dtoC.TotalQuestions = attempt.Items.Count;
            dtoC.TimeLeftSeconds = 0;
            dtoC.TimeLimitSeconds = 0;

            return Result<AttemptDto>.Success(dtoC);
        }

        var total = attempt.Items.Sum(i => i.AwardedScore);
        attempt.Complete(DateTimeOffset.UtcNow, total);

        await uow.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<AttemptDto>(attempt);
        dto.AnsweredCount = attempt.Items.Count;
        dto.TotalQuestions = attempt.Items.Count;
        dto.TimeLeftSeconds = 0;
        dto.TimeLimitSeconds = 0;

        return Result<AttemptDto>.Success(dto);
    }

}
