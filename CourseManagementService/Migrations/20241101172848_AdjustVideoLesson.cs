using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AdjustVideoLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicVideoURL",
                table: "VideoLessons");

            migrationBuilder.AlterColumn<string>(
                name: "UploadStatus",
                table: "VideoLessons",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailURL",
                table: "VideoLessons",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "BaseBlobURL",
                table: "VideoLessons",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "URL without SAS token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseBlobURL",
                table: "VideoLessons");

            migrationBuilder.AlterColumn<int>(
                name: "UploadStatus",
                table: "VideoLessons",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailURL",
                table: "VideoLessons",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicVideoURL",
                table: "VideoLessons",
                type: "text",
                nullable: true);
        }
    }
}
