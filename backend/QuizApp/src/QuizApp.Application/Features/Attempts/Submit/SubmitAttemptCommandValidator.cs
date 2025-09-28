using FluentValidation;

namespace QuizApp.Application.Features.Attempts.Submit;

public class SubmitAttemptCommandValidator : AbstractValidator<SubmitAttemptCommand>
{
    public SubmitAttemptCommandValidator()
    {
        RuleFor(x => x.AttemptId).NotEmpty();
    }

}
