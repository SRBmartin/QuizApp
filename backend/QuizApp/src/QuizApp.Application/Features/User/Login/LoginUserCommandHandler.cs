using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.User.Auth;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.User.Login;

public class LoginUserCommandHandler (
    IUserRepository userRepository,
    IPasswordService passwordService,
    IIdentityService identityService
) : IRequestHandler<LoginUserCommand, Result<AuthDto>>
{
    public async Task<Result<AuthDto>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var identifier = command.UsernameOrEmail.Trim().ToLower();

        var user = await userRepository.FindByUsernameOrEmail(identifier, cancellationToken);

        if (user is null)
            return Result<AuthDto>.Failure(new Error("auth.invalid_credentials", "Invalid username/email or password."));

        if (!passwordService.Verify(command.Password, user.PasswordHash))
            return Result<AuthDto>.Failure(new Error("auth.invalid_credentials", "Invalid username/email or password."));

        var token = await identityService.GenerateAccessTokenAsync(user.Id, user.Username, user.Role.ToString(), cancellationToken);

        return Result<AuthDto>.Success(new(token));
    }
}
