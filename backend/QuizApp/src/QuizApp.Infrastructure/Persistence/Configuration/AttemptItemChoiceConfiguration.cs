using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configuration;

public class AttemptItemChoiceConfiguration : IEntityTypeConfiguration<AttemptItemChoice>
{
    public void Configure(EntityTypeBuilder<AttemptItemChoice> builder)
    {
        builder.ToTable("AttemptItemChoices");
        builder.HasKey(x => new { x.AttemptItemId, x.ChoiceId });

        builder.HasOne(x => x.Choice)
            .WithMany(x => x.AttemptItemChoices)
            .HasForeignKey(x => x.ChoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AttemptItem)
            .WithMany(x => x.SelectedChoices)
            .HasForeignKey(x => x.AttemptItemId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}