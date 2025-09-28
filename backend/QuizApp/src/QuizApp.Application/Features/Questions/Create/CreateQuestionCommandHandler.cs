using AutoMapper;
using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Mappings;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Questions;
using QuizApp.Application.DTOs.Questions.Emums;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Questions.Create;

public class CreateQuestionCommandHandler (
    IQuizRepository quizRepository,
    IQuizQuestionRepository quizQuestionRepository,
    IUnitOfWork uow,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<CreateQuestionCommand, Result<QuestionDto>>
{
    public async Task<Result<QuestionDto>> Handle(CreateQuestionCommand command, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<QuestionDto>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var quiz = await quizRepository.FindByIdAsync(command.QuizId, cancellationToken);
        if (quiz is null)
            return Result<QuestionDto>.Failure(new Error("quiz.not_found", "Quiz not found."));

        var now = DateTimeOffset.UtcNow;

        var question = QuizQuestion.Create(Guid.NewGuid(), command.QuizId, currentUser.Id.Value, command.Points, command.Type.ToDomain(), command.Question, now);

        switch (command.Type)
        {
            case QuestionType.Single:
            case QuestionType.Multi:
                {
                    foreach (var c in command.Choices)
                    {
                        var choice = QuizQuestionChoice.Create(Guid.NewGuid(), question.Id, c.Label, c.IsCorrect);
                        question.Choices.Add(choice);
                    }

                    break;
                }
            case QuestionType.TrueFalse:
                {
                    var trueIsCorrect = command.IsTrueCorrect!.Value;

                    var tr = QuizQuestionChoice.Create(Guid.NewGuid(), question.Id, "True", trueIsCorrect);
                    var fl = QuizQuestionChoice.Create(Guid.NewGuid(), question.Id, "False", !trueIsCorrect);

                    question.Choices.Add(tr);
                    question.Choices.Add(fl);

                    break;
                }
            case QuestionType.FillIn:
                {
                    question.TextAnswer = QuizQuestionText.Create(Guid.NewGuid(), question.Id, command.TextAnswer!);

                    break;
                }
            default:
                return Result<QuestionDto>.Failure(new Error("question.type_invalid", "Unsupported question type."));
        }

        await quizQuestionRepository.AddAsync(question, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<QuestionDto>(question);

        return Result<QuestionDto>.Success(dto);
    }

}
