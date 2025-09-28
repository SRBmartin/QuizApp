namespace QuizApp.Domain.Entities;

public class QuizQuestionText
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Text { get; set; } = default!;

    #region Navigation

    public virtual QuizQuestion Question { get; set; } = default!;

    #endregion

    public static QuizQuestionText Create(Guid id, Guid questionId, string text)
    {
        return new()
        {
            Id = id,
            QuestionId = questionId,
            Text = text
        };
    }

}
