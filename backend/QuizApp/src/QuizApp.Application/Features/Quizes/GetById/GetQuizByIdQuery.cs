using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;

namespace QuizApp.Application.Features.Quizes.GetById;

public record GetQuizByIdQuery
(
    Guid Id  
) : IRequest<Result<QuizDto>>;