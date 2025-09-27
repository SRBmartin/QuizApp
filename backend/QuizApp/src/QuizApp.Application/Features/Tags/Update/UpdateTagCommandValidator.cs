using FluentValidation;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Tags.Update;

public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator(ITagRepository tagRepository)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(x => x).MustAsync(async (c, ct) => !await tagRepository.ExistsByNameExceptAsync(c.Id, c.Name, ct))
            .WithMessage("Another tag with the same name already exists.");
    }

}
