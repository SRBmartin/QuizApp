using AutoMapper;
using MediatR;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Questions;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;

namespace QuizApp.Application.Features.Questions.Update;

public class UpdateQuestionCommandHandler(
    IQuizQuestionRepository quizQuestionRepository,
    IQuizRepository quizRepository,
    IUnitOfWork uow,
    ICurrentUser currentUser,
    IMapper mapper
) : IRequestHandler<UpdateQuestionCommand, Result<QuestionDto>>
{
    public async Task<Result<QuestionDto>> Handle(UpdateQuestionCommand command, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.Id is null)
            return Result<QuestionDto>.Failure(new Error("auth.unauthorized", "Unauthorized."));

        var question = await quizQuestionRepository.FindByIdAsync(command.Id, cancellationToken);
        if (question is null)
            return Result<QuestionDto>.Failure(new Error("question.not_found", "Question not found."));

        var quiz = await quizRepository.FindByIdAsync(question.QuizId, cancellationToken);
        if (quiz is not null && quiz.IsPublished)
            return Result<QuestionDto>.Failure(new Error("quiz.published", "Quiz is published so questions cannot be modified."));

        question.Question = command.Question;
        question.Points = command.Points;

        var activeChoices = question.Choices
            .Where(c => !c.IsDeleted)
            .ToDictionary(c => c.Id, c => c);

        switch (question.Type)
        {
            case QuestionType.Single:
                {
                    if (command.Choices is null || command.Choices.Count < 2)
                        return Fail("choices.invalid", "At least 2 choices are required for Single.");

                    foreach (var c in command.Choices)
                    {
                        if (!activeChoices.ContainsKey(c.Id))
                            return Fail("choices.invalid_id", $"Choice id '{c.Id}' does not exist on this question.");
                    }

                    foreach (var c in command.Choices)
                    {
                        if (c.Id is Guid id && activeChoices.TryGetValue(id, out var existing))
                        {
                            existing.Label = c.Label;
                            existing.IsCorrect = c.IsCorrect;
                        }
                        else
                        {
                            var created = QuizQuestionChoice.Create(Guid.NewGuid(), question.Id, c.Label, c.IsCorrect);
                            question.Choices.Add(created);
                            activeChoices[created.Id] = created;
                        }
                    }

                    var singleSet = question.Choices.Where(x => !x.IsDeleted).ToList();
                    if (singleSet.Count < 2)
                        return Fail("choices.invalid", "At least 2 choices are required for Single.");
                    if (singleSet.Count(x => x.IsCorrect) != 1)
                        return Fail("choices.invalid", "Exactly one choice must be correct for Single.");

                    break;
                }

            case QuestionType.Multi:
                {
                    if (command.Choices is null || command.Choices.Count < 2)
                        return Fail("choices.invalid", "At least 2 choices are required for Multi.");

                    foreach (var c in command.Choices)
                    {
                        if (!activeChoices.ContainsKey(c.Id))
                            return Fail("choices.invalid_id", $"Choice id '{c.Id}' does not exist on this question.");
                    }

                    foreach (var c in command.Choices)
                    {
                        if (c.Id is Guid id && activeChoices.TryGetValue(id, out var existing))
                        {
                            existing.Label = c.Label;
                            existing.IsCorrect = c.IsCorrect;
                        }
                        else
                        {
                            var created = QuizQuestionChoice.Create(Guid.NewGuid(), question.Id, c.Label, c.IsCorrect);
                            question.Choices.Add(created);
                            activeChoices[created.Id] = created;
                        }
                    }

                    var multiSet = question.Choices.Where(x => !x.IsDeleted).ToList();
                    if (multiSet.Count < 2)
                        return Fail("choices.invalid", "At least 2 choices are required for Multi.");
                    if (!multiSet.Any(x => x.IsCorrect))
                        return Fail("choices.invalid", "At least one choice must be correct for Multi.");

                    break;
                }

            case QuestionType.TrueFalse:
                {
                    if (command.IsTrueCorrect is null)
                        return Fail("isTrueCorrect.required", "IsTrueCorrect must be provided for TrueFalse.");

                    var trueIsCorrect = command.IsTrueCorrect.Value;

                    var trueChoice = question.Choices.FirstOrDefault(x => x.Label == "True" && !x.IsDeleted);
                    var falseChoice = question.Choices.FirstOrDefault(x => x.Label == "False" && !x.IsDeleted);

                    if (trueChoice is null)
                    {
                        trueChoice = QuizQuestionChoice.Create(Guid.NewGuid(), question.Id, "True", trueIsCorrect);
                        question.Choices.Add(trueChoice);
                    }
                    else
                    {
                        trueChoice.IsCorrect = trueIsCorrect;
                    }

                    if (falseChoice is null)
                    {
                        falseChoice = QuizQuestionChoice.Create(Guid.NewGuid(), question.Id, "False", !trueIsCorrect);
                        question.Choices.Add(falseChoice);
                    }
                    else
                    {
                        falseChoice.IsCorrect = !trueIsCorrect;
                    }

                    if (command.Choices is not null)
                    {
                        foreach (var c in command.Choices)
                        {
                            if (c.Id is { } id && activeChoices.TryGetValue(id, out var existing))
                            {
                                existing.Label = c.Label;
                            }
                        }
                    }

                    var tfSet = question.Choices.Where(x => !x.IsDeleted).ToList();
                    if (tfSet.Count(x => x.Label == "True" || x.Label == "False") < 2)
                        return Fail("choices.invalid", "True/False must contain exactly two options.");
                    if (tfSet.Count(x => x.IsCorrect) != 1)
                        return Fail("choices.invalid", "Exactly one of True/False must be correct.");

                    break;
                }

            case QuestionType.FillIn:
                {
                    if (string.IsNullOrWhiteSpace(command.TextAnswer))
                        return Fail("textAnswer.required", "TextAnswer is required for FillIn.");
                    if (command.TextAnswer.Length > 2048)
                        return Fail("textAnswer.too_long", "TextAnswer cannot exceed 2048 characters.");

                    if (question.TextAnswer is null)
                        question.TextAnswer = QuizQuestionText.Create(Guid.NewGuid(), question.Id, command.TextAnswer);
                    else
                        question.TextAnswer.Text = command.TextAnswer;

                    break;
                }
        }

        await uow.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<QuestionDto>(question);
        return Result<QuestionDto>.Success(dto);

        static Result<QuestionDto> Fail(string code, string message)
            => Result<QuestionDto>.Failure(new Error(code, message));
    }
}