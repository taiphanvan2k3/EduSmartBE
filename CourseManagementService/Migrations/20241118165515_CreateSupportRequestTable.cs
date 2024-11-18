using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementService.Migrations
{
    /// <inheritdoc />
    public partial class CreateSupportRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromUserId = table.Column<int>(type: "integer", nullable: false, comment: "UserId proceed the request"),
                    ToUserId = table.Column<int>(type: "integer", nullable: false, comment: "UserId to proceed the request, such as TeacherId, AdminId"),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Response = table.Column<string>(type: "text", nullable: true),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_FromUserId_Status",
                table: "SupportRequests",
                columns: new[] { "FromUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_FromUserId_Type",
                table: "SupportRequests",
                columns: new[] { "FromUserId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_ToUserId_Status",
                table: "SupportRequests",
                columns: new[] { "ToUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_ToUserId_Type",
                table: "SupportRequests",
                columns: new[] { "ToUserId", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportRequests");
        }
    }
}
