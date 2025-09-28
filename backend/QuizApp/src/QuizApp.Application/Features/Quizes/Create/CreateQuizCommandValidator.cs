using FluentValidation;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Quizes.Create;

public class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator(IQuizRepository quizRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty().MinimumLength(3).MaximumLength(128);

        RuleFor(x => x.TimeInSeconds)
            .GreaterThanOrEqualTo(30);

        RuleFor(x => x.Name)
            .MustAsync(async (name, ct) => !await quizRepository.ExistsByNameAsync(name, ct))
            .WithMessage("Quiz name already exists.");
    }

}
