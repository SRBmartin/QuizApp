using AutoMapper;
using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Application.DTOs.Tags;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Quizes.GetById;

public class GetQuizByIdQueryHandler (
    IQuizRepository quizRepository,
    IQuizTagRepository quizTagRepository,
    IMapper mapper
) : IRequestHandler<GetQuizByIdQuery, Result<QuizDto>>
{
    public async Task<Result<QuizDto>> Handle(GetQuizByIdQuery query, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.FindByIdAsync(query.Id, cancellationToken);
        if (quiz is null)
            return Result<QuizDto>.Failure(new Error("quiz.not_found", "Quiz not found."));

        var dto = mapper.Map<QuizDto>(quiz);

        var tags = await quizTagRepository.GetTagsForQuizAsync(quiz.Id, cancellationToken);
        dto.Tags = tags.Select(t => mapper.Map<TagDto>(t)).ToList();

        return Result<QuizDto>.Success(dto);
    }

}
