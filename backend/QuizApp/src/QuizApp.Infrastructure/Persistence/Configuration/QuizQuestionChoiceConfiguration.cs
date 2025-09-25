using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configuration;

public class QuizQuestionChoiceConfiguration : IEntityTypeConfiguration<QuizQuestionChoice>
{
    public void Configure(EntityTypeBuilder<QuizQuestionChoice> builder)
    {
        builder.ToTable("QuizQuestionChoices");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Label).IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}