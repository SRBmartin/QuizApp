using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;

namespace QuizApp.Application.Features.Attempts.Start;

public record StartAttemptCommand 
(
    Guid QuizId  
) : IRequest<Result<AttemptQuestionDto>>;