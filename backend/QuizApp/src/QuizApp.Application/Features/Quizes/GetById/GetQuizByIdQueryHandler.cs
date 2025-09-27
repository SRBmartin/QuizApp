using AutoMapper;
using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Quizes.GetById;

public class GetQuizByIdQueryHandler (
    IQuizRepository quizRepository,
    IMapper mapper
) : IRequestHandler<GetQuizByIdQuery, Result<QuizDto>>
{
    public async Task<Result<QuizDto>> Handle(GetQuizByIdQuery query, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.FindByIdAsync(query.Id, cancellationToken);
        if (quiz is null)
            return Result<QuizDto>.Failure(new Error("quiz.not_found", "Quiz not found."));

        var dto = mapper.Map<QuizDto>(quiz);

        return Result<QuizDto>.Success(dto);
    }

}
