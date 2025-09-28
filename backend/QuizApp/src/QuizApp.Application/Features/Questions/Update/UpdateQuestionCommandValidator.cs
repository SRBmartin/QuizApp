using FluentValidation;

namespace QuizApp.Application.Features.Questions.Update;

public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Question)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(1024);

        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0);

        When(x => x.Choices is not null, () =>
        {
            RuleForEach(x => x.Choices!).ChildRules(choices =>
            {
                choices.RuleFor(c => c.Label)
                    .NotEmpty()
                    .MaximumLength(512);
            });
        });

        When(x => x.TextAnswer is not null, () =>
        {
            RuleFor(x => x.TextAnswer!)
                .MaximumLength(2048);
        });
    }

}
