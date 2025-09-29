using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Api.Contracts.Attempts;
using QuizApp.Api.Extensions;
using QuizApp.Api.Web.Auth;
using QuizApp.Application.Features.Attempts.GetAttemptResultCombined;
using QuizApp.Application.Features.Attempts.GetAttemptResultReview;
using QuizApp.Application.Features.Attempts.GetAttemptResultSummary;
using QuizApp.Application.Features.Attempts.GetAttemptState;
using QuizApp.Application.Features.Attempts.GetMyAttemptsQuery;
using QuizApp.Application.Features.Attempts.SaveAnswer;
using QuizApp.Application.Features.Attempts.Start;
using QuizApp.Application.Features.Attempts.Submit;
using QuizApp.Domain.Enums;

namespace QuizApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttemptController (
    IMediator mediator    
) : ControllerBase
{
    [HttpPost("quizzes/{quizId:guid}/attempts")]
    [RequireJwtToken]
    public async Task<IActionResult> StartAttempt([FromRoute] Guid quizId, CancellationToken cancellationToken)
    {
        var command = new StartAttemptCommand(quizId);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("attempts/{attemptId:guid}")]
    [RequireJwtToken]
    public async Task<IActionResult> GetState([FromRoute] Guid attemptId, CancellationToken cancellationToken)
    {
        var query = new GetAttemptStateQuery(attemptId);

        var result = await mediator.Send(query, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("attempts/{attemptId:guid}/next")]
    [RequireJwtToken]
    public async Task<IActionResult> NextQuestion([FromRoute] Guid attemptId, CancellationToken cancellationToken)
    {
        var query = new GetAttemptStateQuery(attemptId);
        var resumeResult = await mediator.Send(query, cancellationToken);
        if (!resumeResult.IsSuccess)
        {
            return this.ToActionResult(resumeResult);
        }

        var state = resumeResult.Value!;
        var command = new StartAttemptCommand(state.QuizId);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("attempts/{attemptId:guid}/questions/{questionId:guid}/answer")]
    [RequireJwtToken]
    public async Task<IActionResult> SaveAnswer([FromRoute] Guid attemptId, [FromRoute] Guid questionId, [FromBody] SaveAnswerRequest request, CancellationToken cancellationToken)
    {
        var command = new SaveAnswerCommand(attemptId, questionId, request.SelectedChoiceIds, request.SubmittedText);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("attempts/{attemptId:guid}/submit")]
    [RequireJwtToken]
    public async Task<IActionResult> Submit([FromRoute] Guid attemptId, CancellationToken cancellationToken)
    {
        var command = new SubmitAttemptCommand(attemptId);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("attempts/{attemptId:guid}/result/summary")]
    [RequireJwtToken]
    public async Task<IActionResult> GetResultSummary([FromRoute] Guid attemptId, CancellationToken cancellationToken)
    {
        var command = new GetAttemptResultSummaryQuery(attemptId);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("attempts/{attemptId:guid}/result/review")]
    [RequireJwtToken]
    public async Task<IActionResult> GetResultReview([FromRoute] Guid attemptId, CancellationToken cancellationToken)
    {
        var command = new GetAttemptResultReviewQuery(attemptId);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("attempts/{attemptId:guid}/result")]
    [RequireJwtToken]
    public async Task<IActionResult> GetResultCombined([FromRoute] Guid attemptId, CancellationToken cancellationToken)
    {
        var command = new GetAttemptResultCombinedQuery(attemptId);

        var result = await mediator.Send(command, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("my")]
    [RequireJwtToken]
    public async Task<IActionResult> GetMyAttempts([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] string? status = null, CancellationToken cancellationToken = default)
    {
        AttemptStatus? parsed = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<AttemptStatus>(status, ignoreCase: true, out var s))
                parsed = s;
            else if (int.TryParse(status, out var si) && Enum.IsDefined(typeof(AttemptStatus), si))
                parsed = (AttemptStatus)si;
        }

        var query = new GetMyAttemptsQuery(skip, take, parsed);

        var result = await mediator.Send(query, cancellationToken);

        return this.ToActionResult(result);
    }

}
