using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories.Writer;
using QuizApp.Infrastructure.Persistence.Context;

namespace QuizApp.Infrastructure.Persistence.Repositories.Writer;

public class AttemptAnswerWriter (
    QuizDbContext dbContext    
) : IAttemptAnswerWriter
{
    public async Task UpsertAsync(
        Guid attemptId,
        Guid questionId,
        IReadOnlyList<Guid>? selectedChoiceIds,
        string? submittedText,
        DateTimeOffset answeredAtUtc,
        CancellationToken ct)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync(ct);

        var attempt = await dbContext.Attempts
            .Select(a => new { a.Id, a.Status, a.StartedAt, a.QuizId })
            .FirstOrDefaultAsync(a => a.Id == attemptId, ct)
            ?? throw new InvalidOperationException("Attempt not found.");

        var question = await dbContext.Set<QuizQuestion>()
            .Include(q => q.Choices)
            .Include(q => q.TextAnswer)
            .FirstOrDefaultAsync(q => q.Id == questionId, ct)
            ?? throw new InvalidOperationException("Question not found.");

        var item = await dbContext.Set<AttemptItem>()
            .FirstOrDefaultAsync(i => i.AttemptId == attemptId && i.QuestionId == questionId, ct);

        if (item is null)
        {
            item = AttemptItem.For(Guid.NewGuid(), attemptId, questionId, answeredAtUtc, 0, false);
            await dbContext.Set<AttemptItem>().AddAsync(item, ct);
            await dbContext.SaveChangesAsync(ct);
        }
        else
        {
            item.AnsweredAt = answeredAtUtc;
            item.AwardedScore = 0;
            item.IsCorrect = false;
            dbContext.Set<AttemptItem>().Update(item);
            await dbContext.SaveChangesAsync(ct);
        }

        try
        {
            await dbContext.Set<AttemptItemChoice>()
                .Where(c => c.AttemptItemId == item.Id)
                .ExecuteDeleteAsync(ct);
            await dbContext.Set<AttemptItemText>()
                .Where(t => t.AttemptItemId == item.Id)
                .ExecuteDeleteAsync(ct);
        }
        catch (NotSupportedException)
        {
            var oldChoices = await dbContext.Set<AttemptItemChoice>().Where(c => c.AttemptItemId == item.Id).ToListAsync(ct);
            dbContext.RemoveRange(oldChoices);
            var oldText = await dbContext.Set<AttemptItemText>().FirstOrDefaultAsync(t => t.AttemptItemId == item.Id, ct);
            if (oldText != null) dbContext.Remove(oldText);
            await dbContext.SaveChangesAsync(ct);
        }

        var awarded = 0;
        var correct = false;

        if (question.Type == QuestionType.Single || question.Type == QuestionType.TrueFalse)
        {
            if (selectedChoiceIds is null || selectedChoiceIds.Count != 1)
                throw new InvalidOperationException("Single choice required.");

            var chosenId = selectedChoiceIds[0];
            if (!question.Choices.Any(c => c.Id == chosenId))
                throw new InvalidOperationException("Choice invalid.");

            await dbContext.Set<AttemptItemChoice>().AddAsync(
                new AttemptItemChoice { AttemptItemId = item.Id, ChoiceId = chosenId }, ct);

            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            correct = correctChoice is not null && correctChoice.Id == chosenId;
            awarded = correct ? question.Points : 0;
        }
        else if (question.Type == QuestionType.Multi)
        {
            if (selectedChoiceIds is null || selectedChoiceIds.Count == 0)
                throw new InvalidOperationException("At least one choice required.");

            var set = selectedChoiceIds.ToHashSet();
            var allExist = question.Choices.Count(c => set.Contains(c.Id)) == set.Count;
            if (!allExist) throw new InvalidOperationException("Choice invalid.");

            foreach (var cid in set)
            {
                await dbContext.Set<AttemptItemChoice>().AddAsync(
                    new AttemptItemChoice { AttemptItemId = item.Id, ChoiceId = cid }, ct);
            }

            var correctSet = question.Choices.Where(c => c.IsCorrect).Select(c => c.Id).OrderBy(x => x).ToList();
            var chosenSet = set.OrderBy(x => x).ToList();
            correct = correctSet.SequenceEqual(chosenSet);
            awarded = correct ? question.Points : 0;
        }
        else if (question.Type == QuestionType.FillIn)
        {
            if (string.IsNullOrWhiteSpace(submittedText))
                throw new InvalidOperationException("Text answer required.");

            var trimmed = submittedText.Trim();
            await dbContext.Set<AttemptItemText>().AddAsync(
                new AttemptItemText { AttemptItemId = item.Id, SubmittedText = trimmed }, ct);

            var expected = question.TextAnswer?.Text?.Trim();
            if (!string.IsNullOrEmpty(expected))
                correct = string.Equals(expected, trimmed, StringComparison.OrdinalIgnoreCase);

            awarded = correct ? question.Points : 0;
        }
        else
        {
            throw new InvalidOperationException("Unsupported question type.");
        }

        item.AwardedScore = awarded;
        item.IsCorrect = correct;
        item.AnsweredAt = answeredAtUtc;

        dbContext.Update(item);
        await dbContext.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);
    }
}
