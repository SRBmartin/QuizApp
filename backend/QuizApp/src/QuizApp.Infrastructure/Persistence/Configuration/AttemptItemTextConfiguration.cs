using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configuration;

public class AttemptItemTextConfiguration : IEntityTypeConfiguration<AttemptItemText>
{
    public void Configure(EntityTypeBuilder<AttemptItemText> builder)
    {
        builder.ToTable("AttemptItemTexts");
        builder.HasKey(x => x.AttemptItemId);

        builder.Property(x => x.SubmittedText)
            .HasMaxLength(1024)
            .IsRequired();
    }
}