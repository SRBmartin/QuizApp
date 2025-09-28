using FluentValidation;

namespace QuizApp.Application.Features.Quizes.Delete;

public class DeleteQuizCommandValidator : AbstractValidator<DeleteQuizCommand>
{
    public DeleteQuizCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }

}
