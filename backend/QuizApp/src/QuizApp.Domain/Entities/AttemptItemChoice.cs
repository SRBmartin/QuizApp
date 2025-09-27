namespace QuizApp.Domain.Entities;

public class AttemptItemChoice
{
    public Guid AttemptItemId { get; set; }
    public Guid ChoiceId { get; set; }

    #region Navigation

    public virtual AttemptItem AttemptItem { get; set; } = default!;
    public virtual QuizQuestionChoice Choice { get; set; } = default!;

    #endregion

}
