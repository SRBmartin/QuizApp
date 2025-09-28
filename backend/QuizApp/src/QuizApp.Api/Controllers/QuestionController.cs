using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Api.Contracts.Questions;
using QuizApp.Api.Extensions;
using QuizApp.Api.Web.Auth;
using QuizApp.Application.Features.Questions.Create;

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

}
