using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Api.Contracts.Tags;
using QuizApp.Api.Extensions;
using QuizApp.Api.Web.Auth;
using QuizApp.Application.Features.Tags.Create;
using QuizApp.Application.Features.Tags.Delete;
using QuizApp.Application.Features.Tags.List;
using QuizApp.Application.Features.Tags.Update;

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

    [HttpPut("{id:guid}")]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTagRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand(id, request.Name);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand(id);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result, successStatusCode: StatusCodes.Status204NoContent);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? skip, [FromQuery] int? take, CancellationToken cancellationToken)
    {
        var query = new ListTagsQuery(skip, take);

        var result = await mediator.Send(query, cancellationToken);

        return this.ToActionResult(result);
    }

}
