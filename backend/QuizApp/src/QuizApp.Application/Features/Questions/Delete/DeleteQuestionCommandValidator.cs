using FluentValidation;

namespace QuizApp.Application.Features.Questions.Delete;

public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
{
    public DeleteQuestionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
