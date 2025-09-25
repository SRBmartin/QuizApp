using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuizApp.Domain.Entities;
using System.Reflection;

namespace QuizApp.Infrastructure.Persistence.Context;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<QuizTag> QuizTags => Set<QuizTag>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizQuestionChoice> QuizQuestionChoices => Set<QuizQuestionChoice>();
    public DbSet<QuizQuestionText> QuizQuestionTexts => Set<QuizQuestionText>();
    public DbSet<Attempt> Attempts => Set<Attempt>();
    public DbSet<AttemptItem> AttemptItems => Set<AttemptItem>();
    public DbSet<AttemptItemChoice> AttemptItemChoices => Set<AttemptItemChoice>();
    public DbSet<AttemptItemText> AttemptItemTexts => Set<AttemptItemText>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Quiz");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Throw(CoreEventId.ShadowPropertyCreated));
    }

}
