using FluentValidation;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Quizes.Update;

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizCommandValidator(IQuizRepository quizRepository)
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(128);
        RuleFor(x => x.TimeInSeconds).GreaterThanOrEqualTo(10);
    }
}
