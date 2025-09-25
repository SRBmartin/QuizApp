namespace QuizApp.Domain.Entities;

public class QuizTag
{
    public Guid QuizId { get; set; }
    public Guid TagId { get; set; }

    #region Navigation

    public virtual Quiz Quiz { get; set; } = default!;
    public virtual Tag Tag { get; set; } = default!;

    #endregion

}
