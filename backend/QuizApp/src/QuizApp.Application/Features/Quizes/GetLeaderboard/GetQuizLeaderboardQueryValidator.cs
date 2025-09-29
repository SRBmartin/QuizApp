using FluentValidation;

namespace QuizApp.Application.Features.Quizes.GetLeaderboard;

public class GetQuizLeaderboardQueryValidator : AbstractValidator<GetQuizLeaderboardQuery>
{
    public GetQuizLeaderboardQueryValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.Take).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.Period)
            .Must(p => string.IsNullOrWhiteSpace(p) || p!.Equals("all", StringComparison.OrdinalIgnoreCase)
                       || p!.Equals("month", StringComparison.OrdinalIgnoreCase)
                       || p!.Equals("week", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Period must be all | month | week.");
    }
}
