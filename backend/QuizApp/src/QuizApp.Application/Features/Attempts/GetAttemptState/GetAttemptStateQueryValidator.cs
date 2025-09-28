using FluentValidation;

namespace QuizApp.Application.Features.Attempts.GetAttemptState;

public class GetAttemptStateQueryValidator : AbstractValidator<GetAttemptStateQuery>
{
    public GetAttemptStateQueryValidator()
    {
        RuleFor(x => x.AttemptId).NotEmpty();
    }
}
