using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configuration;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");
        builder.HasKey(x => x.Id);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.TagsCreated)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
