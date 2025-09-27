using FluentValidation;

namespace QuizApp.Application.Features.User.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().MinimumLength(3).MaximumLength(64);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a non-alphanumeric char.");

        RuleFor(x => x.ImageContentType)
            .NotEmpty()
            .Must(ct => ct!.StartsWith("image/"))
            .WithMessage("Invalid image content type.");

    }

}
