using FluentValidation;

namespace QuizApp.Application.Features.Attempts.Start;

public class StartAttemptCommandValidator : AbstractValidator<StartAttemptCommand>
{
    public StartAttemptCommandValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
    }
}
