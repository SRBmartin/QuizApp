using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;

namespace QuizApp.Application.Features.Attempts.SaveAnswer;

public record SaveAnswerCommand
(
    Guid AttemptId,
    Guid QuestionId,
    List<Guid>? SelectedChoiceIds,
    string? SubmittedText
) : IRequest<Result<AttemptQuestionDto>>;