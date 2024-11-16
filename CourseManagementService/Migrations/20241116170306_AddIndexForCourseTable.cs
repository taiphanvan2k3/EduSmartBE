using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForCourseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses",
                column: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses");
        }
    }
}
