using FluentValidation;

namespace QuizApp.Application.Features.Attempts.GetQuizAttempts;

public class GetQuizAttemptsQueryValidator : AbstractValidator<GetQuizAttemptsQuery>
{
    public GetQuizAttemptsQueryValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.Skip).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Take).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
