using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddIconInforInCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "MobileIconInfo",
                table: "Categories",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebIconInfo",
                table: "Categories",
                type: "json",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileIconInfo",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "WebIconInfo",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Categories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
