namespace PaymentService.Databases.Schemas
{
    public class CourseAchievementTemplate : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public int AchievementTemplateId { get; set; }

        public TextStyleInfo CourseNameTextStyle { get; set; }

        public TextStyleInfo StudentNameTextStyle { get; set; }

        public TextStyleInfo DateTextStyle { get; set; }

        public TextStyleInfo TeacherTextStyle { get; set; }

        public virtual AchievementTemplate AchievementTemplate { get; set; }
    }
}