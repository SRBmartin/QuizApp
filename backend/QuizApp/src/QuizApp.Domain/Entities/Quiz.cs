using QuizApp.Domain.Enums;

namespace QuizApp.Domain.Entities;

public class Quiz
{
    public Guid Id { get; set; }
    public Guid CreatedById { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public QuizLevel DifficultyLevel { get; set; }
    public int TimeInSeconds { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDeleted { get; set; }

    #region Navigation

    public virtual User CreatedBy { get; set; } = default!;
    public virtual ICollection<QuizQuestion> Questions { get; set; } = new HashSet<QuizQuestion>();
    public virtual ICollection<QuizTag> QuizTags { get; set; } = new HashSet<QuizTag>();
    public virtual ICollection<Attempt> Attempts { get; set; } = new HashSet<Attempt>();

    #endregion

}
