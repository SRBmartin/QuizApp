using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.User.Auth;

namespace QuizApp.Application.Features.User.Login;

public record LoginUserCommand
(
    string UsernameOrEmail,
    string Password
) : IRequest<Result<AuthDto>>;