namespace QuizApp.Domain.Entities;

public class AttemptItemText
{
    public Guid AttemptItemId { get; set; }
    public string SubmittedText { get; set; } = default!;

    #region Navigation

    public virtual AttemptItem AttemptItem { get; set; } = default!;

    #endregion

}
