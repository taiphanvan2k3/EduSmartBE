using CourseManagementService.Database.Schemas.DiscussionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagementService.Database.Configurations
{
    public class DiscussionTypeConfiguration : IEntityTypeConfiguration<DiscussionType>
    {
        public void Configure(EntityTypeBuilder<DiscussionType> builder)
        {
            builder.Property(dt => dt.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasMany(dt => dt.Discussions)
                .WithOne(d => d.Type)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}