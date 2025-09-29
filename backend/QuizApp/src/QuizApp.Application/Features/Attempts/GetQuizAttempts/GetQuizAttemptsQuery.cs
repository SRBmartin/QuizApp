using MediatR;
using QuizApp.Application.Common.Page;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.Features.Attempts.GetQuizAttempts;

public record GetQuizAttemptsQuery
(
    Guid QuizId,
    int Skip,
    int Take,
    AttemptStatus? StatusFilter = null,
    Guid? UserId = null
) : IRequest<Result<PagedListDto<QuizAttemptListItemDto>>>;