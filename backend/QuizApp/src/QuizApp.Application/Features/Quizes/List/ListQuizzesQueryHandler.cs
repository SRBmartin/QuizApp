using AutoMapper;
using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Page;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Application.DTOs.Tags;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Quizes.List;

public class ListQuizzesQueryHandler (
    IQuizRepository quizRepository,
    IQuizQuestionRepository quizQuestionRepository,
    IQuizTagRepository quizTagRepository,
    IMapper mapper,
    ICurrentUser currentUser
) : IRequestHandler<ListQuizzesQuery, Result<PagedListDto<QuizDto>>>
{
    public async Task<Result<PagedListDto<QuizDto>>> Handle(ListQuizzesQuery query, CancellationToken cancellationToken)
    {
        var skip = Math.Max(query.Skip ?? 0, 0);
        var take = Math.Clamp(query.Take ?? 20, 1, 100);

        var includeUnpublished = currentUser.IsAuthenticated && currentUser.Role!.Equals("Admin");

        var total = await quizRepository.CountAsync(cancellationToken);
        var items = await quizRepository.ListAsync(skip, take, includeUnpublished, query.TagId, query.Difficulty, query.Search, cancellationToken);

        var dtos = items.Select(i => mapper.Map<QuizDto>(i)).ToList();

        var quizIds = items.Select(i => i.Id).ToArray();
        var tagsByQuiz = await quizTagRepository.GetTagsForQuizzesAsync(quizIds, cancellationToken);

        foreach (var dto in dtos)
        {
            var questionsCount = await quizQuestionRepository.CountByQuizIdAsync(dto.Id, cancellationToken);
            dto.QuestionCount = questionsCount;

            if (tagsByQuiz.TryGetValue(dto.Id, out var tagEntities))
                dto.Tags = tagEntities.Select(t => mapper.Map<TagDto>(t)).ToList();
            else
                dto.Tags = new List<TagDto>();
        }

        var page = new PagedListDto<QuizDto>(dtos, total, skip, take);
        return Result<PagedListDto<QuizDto>>.Success(page);
    }

}
