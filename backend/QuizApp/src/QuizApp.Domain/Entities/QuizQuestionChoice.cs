namespace QuizApp.Domain.Entities;

public class QuizQuestionChoice
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Label { get; set; } = default!;
    public bool IsCorrect { get; set; }
    public bool IsDeleted { get; set; }

    #region Navigation

    public virtual QuizQuestion Question { get; set; } = default!;
    public virtual ICollection<AttemptItemChoice> AttemptItemChoices { get; set; } = new HashSet<AttemptItemChoice>();

    #endregion

    public static QuizQuestionChoice Create(Guid id, Guid questionId, string  label, bool isCorrect)
    {
        return new()
        {
            Id = id,
            QuestionId = questionId,
            Label = label,
            IsCorrect = isCorrect,
            IsDeleted = false
        };
    }
    
    public void Delete()
    {
        IsDeleted = true;
    }

}
