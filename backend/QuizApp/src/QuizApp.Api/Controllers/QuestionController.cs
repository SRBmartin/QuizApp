using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Api.Contracts.Questions;
using QuizApp.Api.Extensions;
using QuizApp.Api.Web.Auth;
using QuizApp.Application.Features.Questions.Create;
using QuizApp.Application.Features.Questions.Delete;
using QuizApp.Application.Features.Questions.Update;

namespace QuizApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionController (
    IMediator mediator    
) : ControllerBase
{
    [HttpPost("{quizId:guid}")]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> Create([FromRoute] Guid quizId, [FromBody] CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateQuestionCommand(
            quizId,
            request.Question,
            request.Points,
            request.QuestionType,
            request.Choices?.Select(t => new CreateQuestionCommand.CreateQuestionChoice(t.Label, t.IsCorrect)).ToList(),
            request.IsTrueCorrect,
            request.TextAnswer
        );

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{questionId:guid}")]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> Update([FromRoute] Guid questionId, [FromBody] UpdateQuestionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateQuestionCommand(
            questionId,
            request.Question,
            request.Points,
            request.Choices?.Select(c => new UpdateQuestionCommand.UpdateQuestionChoice(c.Id ?? default!, c.Label, c.IsCorrect)).ToList(),
            request.IsTrueCorrect,
            request.TextAnswer
        );

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{questionId:guid}")]
    [RequireJwtToken]
    [RequireAdminRole]
    public async Task<IActionResult> Delete([FromRoute] Guid questionId, CancellationToken cancellationToken)
    {
        var command = new DeleteQuestionCommand(questionId);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

}
