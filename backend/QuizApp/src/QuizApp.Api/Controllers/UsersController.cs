using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Api.Contracts.Users;
using QuizApp.Api.Extensions;
using QuizApp.Application.Features.User.Login;
using QuizApp.Application.Features.User.Register;

namespace QuizApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController (
    IMediator mediator    
) : ControllerBase
{
    [HttpPost("register")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Register([FromForm] RegisterForm request, CancellationToken cancellationToken)
    {
        Stream? img = request.Image?.OpenReadStream();

        var command = new RegisterUserCommand(request.Username, request.Email, request.Password, img, request.Image?.ContentType!);


        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.UsernameOrEmail, request.Password);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

}
