using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configuration;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.TimeInSeconds).IsRequired();

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.QuizzesCreated)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Questions)
            .WithOne(x => x.Quiz)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Attempts)
            .WithOne(x => x.Quiz)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}