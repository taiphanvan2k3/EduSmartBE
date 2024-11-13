using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AdjustCourseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BriefDescription",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "DetailedDescription",
                table: "Courses",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "CoreValues",
                table: "Courses",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prerequisites",
                table: "Courses",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviewVideoURL",
                table: "Courses",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoreValues",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Prerequisites",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "PreviewVideoURL",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Courses",
                newName: "DetailedDescription");

            migrationBuilder.AddColumn<string>(
                name: "BriefDescription",
                table: "Courses",
                type: "text",
                nullable: true);
        }
    }
}
