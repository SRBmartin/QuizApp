using AutoMapper;
using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Quizes.Create;

public class CreateQuizCommandHandler (
    IQuizRepository quizRepository,
    IUnitOfWork uow,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<CreateQuizCommand, Result<QuizDto>>
{
    public async Task<Result<QuizDto>> Handle(CreateQuizCommand command, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<QuizDto>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var quiz = Quiz.Create(
            Guid.NewGuid(),
            currentUser.Id.Value,
            command.Name,
            command.DifficultyLevel,
            command.TimeInSeconds,
            command.Description
        );

        await quizRepository.AddAsync(quiz, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<QuizDto>(quiz);

        return Result<QuizDto>.Success(dto);
    }

}
