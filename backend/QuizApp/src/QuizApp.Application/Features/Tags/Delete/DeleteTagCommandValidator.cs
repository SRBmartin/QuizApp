using FluentValidation;

namespace QuizApp.Application.Features.Tags.Delete;

public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
