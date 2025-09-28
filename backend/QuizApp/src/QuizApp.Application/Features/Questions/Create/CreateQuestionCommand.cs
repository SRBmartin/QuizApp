using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Questions;
using QuizApp.Application.DTOs.Questions.Emums;
using static QuizApp.Application.Features.Questions.Create.CreateQuestionCommand;

namespace QuizApp.Application.Features.Questions.Create;

public record CreateQuestionCommand
(
    Guid QuizId,
    string Question,
    int Points,
    QuestionType Type,
    List<CreateQuestionChoice>? Choices,
    bool? IsTrueCorrect,
    string? TextAnswer
) : IRequest<Result<QuestionDto>>
{
    public record CreateQuestionChoice(string Label, bool IsCorrect);
}