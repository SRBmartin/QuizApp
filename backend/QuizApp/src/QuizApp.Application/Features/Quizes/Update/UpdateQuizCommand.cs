using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.Features.Quizes.Update;

public record UpdateQuizCommand
(
    Guid Id,
    string Name,
    string? Description,
    QuizLevel DifficultyLevel,
    int TimeInSeconds,
    bool IsPublished
) : IRequest<Result<QuizDto>>;