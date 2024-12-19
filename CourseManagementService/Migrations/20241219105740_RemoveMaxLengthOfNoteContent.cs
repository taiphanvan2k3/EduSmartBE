using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMaxLengthOfNoteContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Notes",
                type: "text",
                nullable: true,
                comment: "Note about the highlighted text",
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true,
                oldComment: "Note about the highlighted text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Notes",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                comment: "Note about the highlighted text",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComment: "Note about the highlighted text");
        }
    }
}
