using AutoMapper;
using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Page;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Quizes.List;

public class ListQuizzesQueryHandler (
    IQuizRepository quizRepository,
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
        var items = await quizRepository.ListAsync(skip, take, includeUnpublished, cancellationToken);

        var dtos = items.Select(i => mapper.Map<QuizDto>(i)).ToList();

        var page = new PagedListDto<QuizDto>(dtos, total, skip, take);
        return Result<PagedListDto<QuizDto>>.Success(page);
    }

}
