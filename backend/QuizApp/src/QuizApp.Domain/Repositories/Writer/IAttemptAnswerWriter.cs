namespace QuizApp.Domain.Repositories.Writer;

public interface IAttemptAnswerWriter
{
    Task UpsertAsync(
        Guid attemptId,
        Guid questionId,
        IReadOnlyList<Guid>? selectedChoiceIds,
        string? submittedText,
        DateTimeOffset answeredAtUtc,
        CancellationToken cancellationToken);
}
