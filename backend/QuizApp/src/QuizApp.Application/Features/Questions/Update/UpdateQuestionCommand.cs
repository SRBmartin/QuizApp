using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Questions;
using static QuizApp.Application.Features.Questions.Update.UpdateQuestionCommand;

namespace QuizApp.Application.Features.Questions.Update;

public record UpdateQuestionCommand
(
    Guid Id,
    string Question,
    int Points,
    List<UpdateQuestionChoice>? Choices,
    bool? IsTrueCorrect,
    string? TextAnswer
) : IRequest<Result<QuestionDto>>
{
    public record UpdateQuestionChoice(Guid Id, string Label, bool IsCorrect);
}