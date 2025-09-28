using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Api.Contracts.Quizzes;
using QuizApp.Api.Extensions;
using QuizApp.Api.Web.Auth;
using QuizApp.Application.Features.Quizes.Create;
using QuizApp.Application.Features.Quizes.Delete;
using QuizApp.Application.Features.Quizes.GetById;
using QuizApp.Application.Features.Quizes.List;
using QuizApp.Application.Features.Quizes.Update;
using QuizApp.Application.Features.Quizes.UpdateTags;

namespace QuizApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuizController (
    IMediator mediator   
) : ControllerBase
{
    [HttpPost]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> Create([FromBody] CreateQuizRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateQuizCommand(request.Name, request.Description, request.DifficultyLevel, request.TimeInSeconds);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateQuizRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateQuizCommand(id, request.Name, request.Description, request.DifficultyLevel, request.TimeInSeconds, request.IsPublished);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteQuizCommand(id);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetQuizByIdQuery(id);

        var result = await mediator.Send(query, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> ListAll([FromQuery] int? skip, [FromQuery] int? take, CancellationToken cancellationToken)
    {
        var query = new ListQuizzesQuery(skip, take);

        var result = await mediator.Send(query, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{quizId:guid}/tags")]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> UpdateTags([FromRoute] Guid quizId, [FromBody] UpdateQuizTagsRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateQuizTagsCommand(quizId, request.TagIds);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

}
