using FluentValidation;

namespace QuizApp.Application.Features.Quizes.UpdateTags;

public class UpdateQuizTagsCommandValidator : AbstractValidator<UpdateQuizTagsCommand>
{
    public UpdateQuizTagsCommandValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.TagIds)
            .NotNull()
            .Must(list => list.Distinct().Count() == list.Count)
            .WithMessage("Duplicate tag ids are not allowed.");
    }

}
