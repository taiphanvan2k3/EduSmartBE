using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class ModifyVideoLessonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StorageSize",
                table: "VideoLessons",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "TimeSpent",
                table: "LessonTrackings",
                type: "bigint",
                nullable: false,
                comment: "The time spent on the lesson in seconds",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "The time spent on the lesson in minutes");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonTrackings_Courses_CourseId",
                table: "LessonTrackings",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonTrackings_Courses_CourseId",
                table: "LessonTrackings");

            migrationBuilder.DropColumn(
                name: "StorageSize",
                table: "VideoLessons");

            migrationBuilder.AlterColumn<long>(
                name: "TimeSpent",
                table: "LessonTrackings",
                type: "bigint",
                nullable: false,
                comment: "The time spent on the lesson in minutes",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "The time spent on the lesson in seconds");
        }
    }
}
