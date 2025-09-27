using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Api.Contracts.Tags;
using QuizApp.Api.Extensions;
using QuizApp.Api.Web.Auth;
using QuizApp.Application.Features.Tags.Create;

namespace QuizApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagController (
    IMediator mediator    
) : ControllerBase
{

    [HttpPost]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand(request.Name);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

}
