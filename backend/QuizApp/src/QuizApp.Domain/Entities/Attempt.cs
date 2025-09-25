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

}
