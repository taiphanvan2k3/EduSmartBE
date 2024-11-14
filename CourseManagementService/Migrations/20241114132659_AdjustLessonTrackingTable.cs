using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AdjustLessonTrackingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LessonTrackings_LessonId",
                table: "LessonTrackings");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "LessonTrackings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LessonTrackings_CourseId_StudentId",
                table: "LessonTrackings",
                columns: new[] { "CourseId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_LessonTrackings_LessonId_StudentId",
                table: "LessonTrackings",
                columns: new[] { "LessonId", "StudentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LessonTrackings_CourseId_StudentId",
                table: "LessonTrackings");

            migrationBuilder.DropIndex(
                name: "IX_LessonTrackings_LessonId_StudentId",
                table: "LessonTrackings");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "LessonTrackings");

            migrationBuilder.CreateIndex(
                name: "IX_LessonTrackings_LessonId",
                table: "LessonTrackings",
                column: "LessonId");
        }
    }
}
