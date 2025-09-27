using FluentValidation;

namespace QuizApp.Application.Features.Quizes.List;

public class ListQuizzesQueryValidator : AbstractValidator<ListQuizzesQuery>
{
    public ListQuizzesQueryValidator()
    {
        RuleFor(x => x.Skip).GreaterThanOrEqualTo(0).When(x => x.Skip.HasValue);
        RuleFor(x => x.Take).GreaterThan(0).LessThanOrEqualTo(100).When(x => x.Take.HasValue);
    }

}
