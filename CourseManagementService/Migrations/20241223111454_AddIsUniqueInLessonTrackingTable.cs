using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddIsUniqueInLessonTrackingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LessonTrackings_LessonId_StudentId",
                table: "LessonTrackings");

            migrationBuilder.CreateIndex(
                name: "IX_LessonTrackings_LessonId_StudentId",
                table: "LessonTrackings",
                columns: new[] { "LessonId", "StudentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LessonTrackings_LessonId_StudentId",
                table: "LessonTrackings");

            migrationBuilder.CreateIndex(
                name: "IX_LessonTrackings_LessonId_StudentId",
                table: "LessonTrackings",
                columns: new[] { "LessonId", "StudentId" });
        }
    }
}
