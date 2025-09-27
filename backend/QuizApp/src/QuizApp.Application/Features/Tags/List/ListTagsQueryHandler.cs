using AutoMapper;
using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Tags;
using QuizApp.Domain.Repositories;

namespace QuizApp.Application.Features.Tags.List;

public class ListTagsQueryHandler (
    ITagRepository tagRepository,
    IMapper mapper
) : IRequestHandler<ListTagsQuery, Result<IReadOnlyList<TagDto>>>
{
    public async Task<Result<IReadOnlyList<TagDto>>> Handle(ListTagsQuery query, CancellationToken cancellationToken)
    {
        var tags = await tagRepository.ListAsync(query.Skip, query.Take, cancellationToken);

        var dtos = mapper.Map<IReadOnlyList<TagDto>>(tags);
        
        return Result<IReadOnlyList<TagDto>>.Success(dtos);
    }
}
