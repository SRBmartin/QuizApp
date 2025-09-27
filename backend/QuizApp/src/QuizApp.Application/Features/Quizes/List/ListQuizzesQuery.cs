using MediatR;
using QuizApp.Application.Common.Page;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;

namespace QuizApp.Application.Features.Quizes.List;

public record ListQuizzesQuery
(
    int? Skip,
    int? Take
) : IRequest<Result<PagedListDto<QuizDto>>>;