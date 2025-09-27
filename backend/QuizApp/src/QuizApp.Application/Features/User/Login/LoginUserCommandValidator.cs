using FluentValidation;

namespace QuizApp.Application.Features.User.Login;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty().MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty();
    }

}
