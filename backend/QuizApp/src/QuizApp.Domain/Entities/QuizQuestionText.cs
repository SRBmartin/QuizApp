namespace QuizApp.Domain.Entities;

public class QuizQuestionText
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Text { get; set; } = default!;

    #region Navigation

    public virtual QuizQuestion Question { get; set; } = default!;

    #endregion

}
