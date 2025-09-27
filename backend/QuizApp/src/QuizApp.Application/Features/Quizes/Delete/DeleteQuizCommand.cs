using MediatR;
using QuizApp.Application.Common.Result;

namespace QuizApp.Application.Features.Quizes.Delete;

public record DeleteQuizCommand
(
    Guid Id  
) : IRequest<Result<Unit>>;