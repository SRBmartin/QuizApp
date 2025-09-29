using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;

namespace QuizApp.Application.Features.Attempts.Submit;

public record SubmitAttemptCommand
(
    Guid AttemptId    
) : IRequest<Result<AttemptDto>>;