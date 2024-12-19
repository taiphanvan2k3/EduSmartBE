using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDefaultInCourseAchievementTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TeacherTextStyle",
                table: "CourseAchievementTemplates",
                newName: "TeacherNameTextStyle");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "CourseAchievementTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "CourseAchievementTemplates");

            migrationBuilder.RenameColumn(
                name: "TeacherNameTextStyle",
                table: "CourseAchievementTemplates",
                newName: "TeacherTextStyle");
        }
    }
}
