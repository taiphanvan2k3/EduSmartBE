using CourseManagementService.Database.Schemas.DiscussionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagementService.Database.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Property(c => c.Type)
                .HasConversion<string>();

            builder.HasOne(rep => rep.Parent)
                .WithMany(c => c.Replies)
                .HasForeignKey(rep => rep.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Discussion)
                .WithMany(d => d.Comments)
                .HasForeignKey(c => c.DiscussionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}