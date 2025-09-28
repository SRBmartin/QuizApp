using FluentValidation;
using QuizApp.Application.DTOs.Questions.Emums;

namespace QuizApp.Application.Features.Questions.Create;

public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.Question).NotEmpty().MinimumLength(3).MaximumLength(1024);
        RuleFor(x => x.Points).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Type).IsInEnum();

        When(x => x.Type == QuestionType.Single, () =>
        {
            RuleFor(x => x.Choices)
                .NotNull().WithMessage("Choices are required for SingleChoice.")
                .Must(c => c!.Count >= 2).WithMessage("At least 2 choices are required.")
                .Must(c => c!.Count(ch => ch.IsCorrect) == 1)
                .WithMessage("Exactly one choice must be correct for SingleChoice.");
            RuleForEach(x => x.Choices!).ChildRules(choices =>
            {
                choices.RuleFor(c => c.Label).NotEmpty().MaximumLength(512);
            });
        });

        When(x => x.Type == QuestionType.Multi, () =>
        {
            RuleFor(x => x.Choices)
                .NotNull().WithMessage("Choices are required for MultipleChoice.")
                .Must(c => c!.Count >= 2).WithMessage("At least 2 choices are required.")
                .Must(c => c!.Any(ch => ch.IsCorrect))
                .WithMessage("At least one choice must be correct for MultipleChoice.");
            RuleForEach(x => x.Choices!).ChildRules(choices =>
            {
                choices.RuleFor(c => c.Label).NotEmpty().MaximumLength(512);
            });
        });

        When(x => x.Type == QuestionType.TrueFalse, () =>
        {
            RuleFor(x => x.IsTrueCorrect)
                .NotNull().WithMessage("IsTrueCorrect must be provided for TrueFalse.");
        });

        When(x => x.Type == QuestionType.FillIn, () =>
        {
            RuleFor(x => x.TextAnswer)
                .NotEmpty().WithMessage("TextAnswer is required for Text questions.")
                .MaximumLength(2048);
        });
    }
}
