using QuizApp.Domain.Enums;

namespace QuizApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Email { get; set; } = default!;
    public Role Role { get; set; }
    public string? Photo { get; set; }

    #region Navigation

    public virtual ICollection<Quiz> QuizzesCreated { get; set; } = new HashSet<Quiz>();
    public virtual ICollection<Tag> TagsCreated { get; set; } = new HashSet<Tag>();
    public virtual ICollection<QuizQuestion> QuestionsCreated { get; set; } = new HashSet<QuizQuestion>();
    public virtual ICollection<Attempt> Attempts { get; set; } = new HashSet<Attempt>();

    #endregion

    public static User Create(Guid id, string username, string email, string passwordHash, Role role = Role.User, string? photo = null)
    {
        return new()
        {
            Id = id,
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            Photo = photo
        };
    }

}
