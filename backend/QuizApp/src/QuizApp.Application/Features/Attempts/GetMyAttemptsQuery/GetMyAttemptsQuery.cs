using MediatR;
using QuizApp.Application.Common.Page;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Attempts;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.Features.Attempts.GetMyAttemptsQuery;

public record GetMyAttemptsQuery
(
    int Skip,
    int Take,
    AttemptStatus? StatusFilter = null,
    Guid? QuizId = null
) : IRequest<Result<PagedListDto<MyAttemptListItemDto>>>;