namespace QuizApp.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public Guid CreatedById { get; set; }
    public string Name { get; set; } = default!;

    #region Navigation

    public virtual User CreatedBy { get; set; } = default!;
    public virtual ICollection<QuizTag> QuizTags { get; set; } = new HashSet<QuizTag>();

    #endregion

}
