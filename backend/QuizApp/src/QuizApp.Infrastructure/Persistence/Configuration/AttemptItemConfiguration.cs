using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configuration;

public class AttemptItemConfiguration : IEntityTypeConfiguration<AttemptItem>
{
    public void Configure(EntityTypeBuilder<AttemptItem> builder)
    {
        builder.ToTable("AttemptItems");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.AttemptId, x.QuestionId }).IsUnique();

        builder.HasOne(x => x.Question)
            .WithMany(x => x.AttemptItems)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.SelectedChoices)
            .WithOne(x => x.AttemptItem)
            .HasForeignKey(x => x.AttemptItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TextAnswer)
            .WithOne(x => x.AttemptItem)
            .HasForeignKey<AttemptItemText>(x => x.AttemptItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}