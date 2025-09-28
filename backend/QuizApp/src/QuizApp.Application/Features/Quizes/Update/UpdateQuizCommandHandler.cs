using AutoMapper;
using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Quizes.Update;

public class UpdateQuizCommandHandler (
    IQuizRepository quizRepository,
    IUnitOfWork uow,
    IMapper mapper
) : IRequestHandler<UpdateQuizCommand, Result<QuizDto>>
{
    public async Task<Result<QuizDto>> Handle(UpdateQuizCommand command, CancellationToken cancellationToken)
    {
        var quiz = await quizRepository.FindByIdAsync(command.Id, cancellationToken);
        if (quiz is null)
            return Result<QuizDto>.Failure(new Error("quiz.not_found", "Quiz not found."));

        if (!string.Equals(quiz.Name, command.Name, StringComparison.Ordinal))
        {
            var taken = await quizRepository.ExistsByNameExceptIdAsync(command.Name, command.Id, cancellationToken);
            if (taken)
                return Result<QuizDto>.Failure(new Error("quiz.name_taken", "Quiz name already exists."));
        }

        quiz.Update(command.Name, command.Description, command.DifficultyLevel, command.TimeInSeconds, command.IsPublished);

        await uow.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<QuizDto>(quiz);

        return Result<QuizDto>.Success(dto);
    }

}
