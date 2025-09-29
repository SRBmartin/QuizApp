using FluentValidation;

namespace QuizApp.Application.Features.Attempts.GetAttemptResultSummary;

public class GetAttemptResultSummaryQueryValidator : AbstractValidator<GetAttemptResultSummaryQuery>
{
    public GetAttemptResultSummaryQueryValidator()
    {
        RuleFor(x => x.AttemptId).NotEmpty();
    }
}
