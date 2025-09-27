using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.User.Auth;

namespace QuizApp.Application.Features.User.Register;

public record RegisterUserCommand
(
    string Username,
    string Email,
    string Password,
    Stream ImageContent,
    string ImageContentType
) : IRequest<Result<RegisterUserDto>>;