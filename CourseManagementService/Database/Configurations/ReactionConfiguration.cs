using CourseManagementService.Database.Schemas.DiscussionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagementService.Database.Configurations
{
    public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder.Property(r => r.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(r => r.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(r => r.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}