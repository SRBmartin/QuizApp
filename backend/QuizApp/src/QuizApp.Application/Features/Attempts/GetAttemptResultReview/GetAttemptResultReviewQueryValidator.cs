using FluentValidation;

namespace QuizApp.Application.Features.Attempts.GetAttemptResultReview;

public class GetAttemptResultReviewQueryValidator : AbstractValidator<GetAttemptResultReviewQuery>
{
    public GetAttemptResultReviewQueryValidator()
    {
        RuleFor(x => x.AttemptId).NotEmpty();
    }
}
