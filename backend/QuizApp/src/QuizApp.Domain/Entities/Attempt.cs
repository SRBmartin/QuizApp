using QuizApp.Domain.Enums;

namespace QuizApp.Domain.Entities;

public class Attempt
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
    public int TotalScore { get; set; }
    public AttemptStatus Status { get; set; }

    #region Navigation

    public virtual Quiz Quiz { get; set; } = default!;
    public virtual User User { get; set; } = default!;
    public virtual ICollection<AttemptItem> Items { get; set; } = new HashSet<AttemptItem>();

    #endregion

    public static Attempt Start(Guid id, Guid quizId, Guid userId, DateTimeOffset startedAtUtc)
    {
        return new()
        {
            Id = id,
            QuizId = quizId,
            UserId = userId,
            StartedAt = startedAtUtc,
            Status = AttemptStatus.InProgress,
            TotalScore = 0
        };
    }

    public void Complete(DateTimeOffset submittedAtUtc, int totalScore)
    {
        SubmittedAt = submittedAtUtc;
        Status = AttemptStatus.Completed;
        TotalScore = totalScore;
    }

}
