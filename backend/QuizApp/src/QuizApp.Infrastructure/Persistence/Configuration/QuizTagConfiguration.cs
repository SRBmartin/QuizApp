using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configuration;

public class QuizTagConfiguration : IEntityTypeConfiguration<QuizTag>
{
    public void Configure(EntityTypeBuilder<QuizTag> builder)
    {
        builder.ToTable("QuizTags");
        builder.HasKey(x => new { x.QuizId, x.TagId });

        builder.HasOne(x => x.Quiz)
            .WithMany(x => x.QuizTags)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.QuizTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}