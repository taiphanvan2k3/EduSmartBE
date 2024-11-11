namespace CourseManagementService.Services.CategoryManagement.Schemas
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IconInfoDto WebIconInfo { get; set; }

        public IconInfoDto MobileIconInfo { get; set; }
    }
}