using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddVisibiltyStatusInCourseEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "LessonTrackings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "CourseEnrollments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VisibilityStatus",
                table: "CourseEnrollments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Unknown",
                comment: "Allow other students to see the progress of this student in this course");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_CourseId_StudentId_LeaveDate",
                table: "CourseEnrollments",
                columns: new[] { "CourseId", "StudentId", "LeaveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_StudentId_LeaveDate",
                table: "CourseEnrollments",
                columns: new[] { "StudentId", "LeaveDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseEnrollments_CourseId_StudentId_LeaveDate",
                table: "CourseEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_CourseEnrollments_StudentId_LeaveDate",
                table: "CourseEnrollments");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "LessonTrackings");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "CourseEnrollments");

            migrationBuilder.DropColumn(
                name: "VisibilityStatus",
                table: "CourseEnrollments");
        }
    }
}
