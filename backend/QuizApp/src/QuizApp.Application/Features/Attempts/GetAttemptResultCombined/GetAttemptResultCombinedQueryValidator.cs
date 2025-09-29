using FluentValidation;

namespace QuizApp.Application.Features.Attempts.GetAttemptResultCombined;

public class GetAttemptResultCombinedQueryValidator : AbstractValidator<GetAttemptResultCombinedQuery>
{
    public GetAttemptResultCombinedQueryValidator()
    {
        RuleFor(x => x.AttemptId).NotEmpty();
    }
}
