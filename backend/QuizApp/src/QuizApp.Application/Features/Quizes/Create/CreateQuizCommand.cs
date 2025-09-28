using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.Features.Quizes.Create;

public record CreateQuizCommand 
(
    string Name,
    string? Description,
    QuizLevel DifficultyLevel,
    int TimeInSeconds
) : IRequest<Result<QuizDto>>;