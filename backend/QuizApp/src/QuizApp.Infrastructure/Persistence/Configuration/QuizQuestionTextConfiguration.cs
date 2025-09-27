using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configuration;

public class QuizQuestionTextConfiguration : IEntityTypeConfiguration<QuizQuestionText>
{
    public void Configure(EntityTypeBuilder<QuizQuestionText> builder)
    {
        builder.ToTable("QuizQuestionTexts");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text).IsRequired();
    }
}