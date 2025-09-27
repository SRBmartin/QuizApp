namespace QuizApp.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public Guid CreatedById { get; set; }
    public string Name { get; set; } = default!;
    public bool IsDeleted { get; set; }

    #region Navigation

    public virtual User CreatedBy { get; set; } = default!;
    public virtual ICollection<QuizTag> QuizTags { get; set; } = new HashSet<QuizTag>();

    #endregion

    public static Tag Create(Guid id, string name, Guid CreatedBy)
    {
        return new()
        {
            Id = id,
            Name = name,
            CreatedById = CreatedBy,
            IsDeleted = false
        };
    }

    public void Update(string name)
    {
        Name = name;
    }

    public void Delete()
    {
        IsDeleted = true;
    }

}
