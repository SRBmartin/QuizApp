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

    public static Quiz Create(
        Guid id,
        Guid createdById,
        string name,
        QuizLevel difficultyLevel,
        int timeInSeconds,
        string? description,
        DateTimeOffset? createdAt = null
    )
    {
        return new()
        {
            Id = id,
            CreatedById = createdById,
            Name = name,
            DifficultyLevel = difficultyLevel,
            TimeInSeconds = timeInSeconds,
            Description = description,
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow,
            IsPublished = false,
            IsDeleted = false
        };
    }

    public void Update(string name, string? description, QuizLevel level, int timeInSeconds, bool isPublished)
    {
        Name = name;
        Description = description;
        DifficultyLevel = level;
        TimeInSeconds = timeInSeconds;
        IsPublished = isPublished;
    }

    public void Delete()
    {
        IsDeleted = true;
    }

}
