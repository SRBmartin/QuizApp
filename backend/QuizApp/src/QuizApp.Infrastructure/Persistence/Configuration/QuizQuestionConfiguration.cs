using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configuration;

public class QuizQuestionConfiguration : IEntityTypeConfiguration<QuizQuestion>
{
    public void Configure(EntityTypeBuilder<QuizQuestion> builder)
    {
        builder.ToTable("QuizQuestions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Question).IsRequired();
        builder.Property(x => x.Points).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Quiz)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.QuestionsCreated)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Choices)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TextAnswer)
            .WithOne(x => x.Question)
            .HasForeignKey<QuizQuestionText>(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}