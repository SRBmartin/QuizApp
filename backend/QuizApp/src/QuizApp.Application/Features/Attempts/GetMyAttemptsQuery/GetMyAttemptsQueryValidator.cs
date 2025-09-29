using FluentValidation;

namespace QuizApp.Application.Features.Attempts.GetMyAttemptsQuery;

public class GetMyAttemptsQueryValidator : AbstractValidator<GetMyAttemptsQuery>
{
    public GetMyAttemptsQueryValidator()
    {
        RuleFor(x => x.Skip).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Take).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
