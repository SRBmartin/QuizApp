using FluentValidation;

namespace QuizApp.Application.Features.Attempts.SaveAnswer;

public class SaveAnswerCommandValidator : AbstractValidator<SaveAnswerCommand>
{
    public SaveAnswerCommandValidator()
    {
        RuleFor(x => x.AttemptId).NotEmpty();
        RuleFor(x => x.QuestionId).NotEmpty();
    }
}
