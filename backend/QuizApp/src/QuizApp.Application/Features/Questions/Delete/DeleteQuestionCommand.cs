using MediatR;
using QuizApp.Application.Common.Result;

namespace QuizApp.Application.Features.Questions.Delete;

public record DeleteQuestionCommand
(
    Guid Id  
) : IRequest<Result<Unit>>;