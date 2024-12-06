using CourseManagementService.Database.Schemas;
using CourseManagementService.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace CourseManagementService.Database
{
    public static class ModelCreate
    {
        public static ModelBuilder OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(e => e.Id);

                entity.HasIndex(c => c.TeacherId);

                entity.HasMany(c => c.Chapters)
                    .WithOne(ch => ch.Course)
                    .HasForeignKey(ch => ch.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Tags)
                    .WithOne(ct => ct.Course)
                    .HasForeignKey(ct => ct.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.Type)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(c => c.CoreValues)
                    .HasColumnType("json")
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>()
                    )
                    // Dùng thêm ValueComparer vì EF Core không hỗ trợ so sánh List<string> mặc định
                    // mỗi lần thay đổi dữ liệu, EF Core sẽ coi như dữ liệu đã thay đổi
                    .Metadata.SetValueComparer(
                        new ValueComparer<List<string>>(
                            (c1, c2) => JsonConvert.SerializeObject(c1) == JsonConvert.SerializeObject(c2),
                            c => c == null ? 0 : JsonConvert.SerializeObject(c).GetHashCode(),
                            c => JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(c))!
                        )
                    );

                entity.Property(c => c.Prerequisites)
                    .HasColumnType("json")
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<List<string>>(v) ?? new List<string>()
                    )
                    .Metadata.SetValueComparer(
                        new ValueComparer<List<string>>(
                            (c1, c2) => JsonConvert.SerializeObject(c1) == JsonConvert.SerializeObject(c2),
                            c => c == null ? 0 : JsonConvert.SerializeObject(c).GetHashCode(),
                            c => JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(c))!
                        )
                    );

                entity.Property(c => c.IsPublished)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tags");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<CourseTag>(entity =>
            {
                entity.ToTable("CourseTags");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.Id);

                entity.HasMany(c => c.Courses)
                    .WithOne(c => c.Category)
                    .HasForeignKey(c => c.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.WebIconInfo)
                    .HasColumnType("json")
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<IconInfo>(v)
                    );

                entity.Property(c => c.MobileIconInfo)
                    .HasColumnType("json")
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<IconInfo>(v)
                    );
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.ToTable("Chapters");
                entity.HasKey(e => e.Id);

                entity.HasMany(ch => ch.Lessons)
                    .WithOne(l => l.Chapter)
                    .HasForeignKey(l => l.ChapterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(ch => ch.IsPublished)
                    .HasDefaultValue(true);
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasMany(c => c.Courses)
                    .WithOne(c => c.Currency)
                    .HasForeignKey(c => c.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CourseEnrollment>(entity =>
            {
                entity.ToTable("CourseEnrollments");
                entity.HasKey(e => new { e.CourseId, e.StudentId });

                entity.HasIndex(e => new { e.StudentId, e.LeaveDate });
                entity.HasIndex(e => new { e.CourseId, e.StudentId, e.LeaveDate });

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.VisibilityStatus)
                    .HasDefaultValue(CourseProgressVisibility.Unknown)
                    .HasConversion<string>()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("Lessons");
                entity.HasKey(e => e.Id);

                entity.HasOne(l => l.QuizLesson)
                    .WithOne(ql => ql.Lesson)
                    .HasForeignKey<QuizLesson>(ql => ql.LessonId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(l => l.VideoLesson)
                    .WithOne(vl => vl.Lesson)
                    .HasForeignKey<VideoLesson>(vl => vl.LessonId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(l => l.IsRatingAllowed)
                    .HasDefaultValue(true);
                entity.Property(l => l.IsCommentAllowed)
                    .HasDefaultValue(true);
                entity.Property(l => l.LessonType)
                    .HasConversion<string>()
                    .HasMaxLength(50);
                entity.Property(l => l.DifficultyLevel)
                    .HasConversion<string>()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<QuizLesson>(entity =>
            {
                entity.ToTable("QuizLessons");
                entity.HasKey(e => e.Id);

                entity.HasMany(ql => ql.Answers)
                    .WithOne(qa => qa.QuizLesson)
                    .HasForeignKey(qa => qa.QuizLessonId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VideoLesson>(entity =>
            {
                entity.ToTable("VideoLessons");
                entity.HasKey(e => e.Id);

                entity.Property(vl => vl.UploadStatus)
                    .HasConversion<string>()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<QuizAnswer>(entity =>
            {
                entity.ToTable("QuizAnswers");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<CourseRating>(entity =>
            {
                entity.ToTable("CourseRatings");
                entity.HasKey(e => e.Id);

                entity.HasOne(cr => cr.Course)
                    .WithMany(c => c.Ratings)
                    .HasForeignKey(cr => cr.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LessonRating>(entity =>
            {
                entity.ToTable("LessonRatings");
                entity.HasKey(e => e.Id);

                entity.HasOne(lr => lr.Lesson)
                    .WithMany(l => l.Ratings)
                    .HasForeignKey(lr => lr.LessonId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LessonTracking>(entity =>
            {
                entity.ToTable("LessonTrackings");
                entity.HasKey(e => e.Id);

                entity.HasOne(lt => lt.Lesson)
                    .WithMany(l => l.LessonTrackings)
                    .HasForeignKey(lt => lt.LessonId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(lt => lt.Course)
                    .WithMany(c => c.LessonTrackings)
                    .HasForeignKey(lt => lt.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(lt => new { lt.CourseId, lt.StudentId });
                entity.HasIndex(lt => new { lt.LessonId, lt.StudentId });
            });

            modelBuilder.Entity<SupportRequest>(entity =>
            {
                entity.ToTable("SupportRequests");
                entity.HasKey(e => e.Id);

                entity.Property(sr => sr.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(sr => sr.Type)
                    .HasConversion<string>()
                    .HasMaxLength(100);

                entity.HasIndex(sr => new { sr.FromUserId, sr.Status });
                entity.HasIndex(sr => new { sr.ToUserId, sr.Status });
                entity.HasIndex(sr => new { sr.FromUserId, sr.Type });
                entity.HasIndex(sr => new { sr.ToUserId, sr.Type });
            });

            return modelBuilder;
        }

        public static void ConfigureForBaseEntity(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var createdAtProperty = entityType.ClrType.GetProperty("CreatedAt");
                if (createdAtProperty != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(createdAtProperty.Name)
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .ValueGeneratedOnAdd()
                        .IsRequired();
                }
            }
        }
    }
}