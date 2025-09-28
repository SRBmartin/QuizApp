using MediatR;
using QuizApp.Application.Common.Page;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.Features.Quizes.List;

public record ListQuizzesQuery
(
    int? Skip,
    int? Take,
    Guid? TagId,
    QuizLevel? Difficulty,
    string? Search
) : IRequest<Result<PagedListDto<QuizDto>>>;