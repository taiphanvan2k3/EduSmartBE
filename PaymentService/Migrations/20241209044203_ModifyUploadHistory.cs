using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Migrations
{
    /// <inheritdoc />
    public partial class ModifyUploadHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UploadHistories",
                newName: "UploadDate");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "UploadHistories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "UploadHistories",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "UploadHistories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "CourseId, VideoLessonId, AttachmentId, CommentId...");

            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "UploadHistories",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "The resource type that the file is uploaded for.");

            migrationBuilder.CreateIndex(
                name: "IX_UploadHistories_ResourceType_ResourceId",
                table: "UploadHistories",
                columns: new[] { "ResourceType", "ResourceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UploadHistories_ResourceType_ResourceId",
                table: "UploadHistories");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "UploadHistories");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "UploadHistories");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "UploadHistories");

            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "UploadHistories");

            migrationBuilder.RenameColumn(
                name: "UploadDate",
                table: "UploadHistories",
                newName: "CreatedAt");
        }
    }
}
