using FluentValidation;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Tags.Create;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator(ITagRepository tagRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().MinimumLength(2).MaximumLength(64);

        RuleFor(x => x.Name)
            .MustAsync(async (name, ct) => !await tagRepository.ExistsByNameAsync(name, ct))
            .WithMessage("Tag name already exists.");
    }

}
