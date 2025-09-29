namespace QuizApp.Domain.Entities;

public class AttemptItem
{
    public Guid Id { get; set; }
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public int AwardedScore { get; set; }
    public bool IsCorrect { get; set; }
    public DateTimeOffset? AnsweredAt { get; set; }

    #region Navigation

    public virtual Attempt Attempt { get; set; } = default!;
    public virtual QuizQuestion Question { get; set; } = default!;
    public virtual ICollection<AttemptItemChoice> SelectedChoices { get; set; } = new HashSet<AttemptItemChoice>();
    public virtual AttemptItemText? TextAnswer { get; set; }

    #endregion

    public static AttemptItem For(Guid id, Guid attemptId, Guid questionId, DateTimeOffset answeredAtUtc, int awardedScore, bool isCorrect)
    {
        return new()
        {
            Id = id,
            AttemptId = attemptId,
            QuestionId = questionId,
            AnsweredAt = answeredAtUtc,
            AwardedScore = awardedScore,
            IsCorrect = isCorrect
        };
    }

}
