using FluentValidation;

namespace QuizApp.Application.Features.Tags.List;

public class ListTagsQueryValidator : AbstractValidator<ListTagsQuery>
{
    public ListTagsQueryValidator()
    {
        RuleFor(x => x.Take).GreaterThan(0).When(x => x.Take.HasValue);
        RuleFor(x => x.Skip).GreaterThanOrEqualTo(0).When(x => x.Skip.HasValue);
    }

}
