using QuizApp.Domain.Enums;

namespace QuizApp.Domain.Entities;

public class QuizQuestion
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Guid CreatedById { get; set; }
    public int Points { get; set; }
    public QuestionType Type { get; set; }
    public string Question { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    #region Navigation

    public virtual Quiz Quiz { get; set; } = default!;
    public virtual User CreatedBy { get; set; } = default!;
    public virtual ICollection<QuizQuestionChoice> Choices { get; set; } = new HashSet<QuizQuestionChoice>();
    public virtual QuizQuestionText? TextAnswer { get; set; }
    public virtual ICollection<AttemptItem> AttemptItems { get; set; } = new HashSet<AttemptItem>();

    #endregion

    public static QuizQuestion Create(
        Guid id,
        Guid quizId,
        Guid createdById,
        int points,
        QuestionType questionType,
        string question,
        DateTimeOffset createdAt
    )
    {
        return new()
        {
            Id = id,
            QuizId = quizId,
            CreatedById = createdById,
            Points = points,
            Type = questionType,
            Question = question,
            CreatedAt = createdAt,
            IsDeleted = false
        };
    }

    public void Delete()
    {
        IsDeleted = true;

        foreach (var c in Choices)
        {
            c.Delete();
        }
    }

}
