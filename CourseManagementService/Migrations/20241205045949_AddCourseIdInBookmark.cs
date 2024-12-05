using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseIdInBookmark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Bookmarks");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "Bookmarks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_CourseId",
                table: "Bookmarks",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_UserId_CourseId",
                table: "Bookmarks",
                columns: new[] { "UserId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_UserId_LessonId",
                table: "Bookmarks",
                columns: new[] { "UserId", "LessonId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Courses_CourseId",
                table: "Bookmarks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Courses_CourseId",
                table: "Bookmarks");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_CourseId",
                table: "Bookmarks");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_UserId_CourseId",
                table: "Bookmarks");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_UserId_LessonId",
                table: "Bookmarks");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Bookmarks");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Bookmarks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
