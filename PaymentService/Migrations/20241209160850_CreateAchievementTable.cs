using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PaymentService.Migrations
{
    /// <inheritdoc />
    public partial class CreateAchievementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ThumbnailURL = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TemplateURL = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseAchievementTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementTemplateId = table.Column<int>(type: "integer", nullable: false),
                    CourseNameTextStyle = table.Column<string>(type: "jsonb", nullable: true),
                    StudentNameTextStyle = table.Column<string>(type: "jsonb", nullable: true),
                    DateTextStyle = table.Column<string>(type: "jsonb", nullable: true),
                    TeacherTextStyle = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAchievementTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseAchievementTemplates_AchievementTemplates_Achievement~",
                        column: x => x.AchievementTemplateId,
                        principalTable: "AchievementTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseAchievementTemplates_AchievementTemplateId",
                table: "CourseAchievementTemplates",
                column: "AchievementTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAchievementTemplates_CourseId_AchievementTemplateId",
                table: "CourseAchievementTemplates",
                columns: new[] { "CourseId", "AchievementTemplateId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseAchievementTemplates");

            migrationBuilder.DropTable(
                name: "AchievementTemplates");
        }
    }
}
