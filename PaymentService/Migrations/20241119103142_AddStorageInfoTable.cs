using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PaymentService.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageInfoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RelatedInformation",
                table: "PaymentTransactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "json",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PaymentTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StorageInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    MaximumStorage = table.Column<long>(type: "bigint", nullable: false),
                    UsedStorage = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtendStorages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StorageAmount = table.Column<long>(type: "bigint", nullable: false, comment: "The amount of storage that is bought"),
                    Price = table.Column<decimal>(type: "numeric", nullable: false, comment: "The price of the storage at the time of purchase"),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    BoughtAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StorageInfoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtendStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtendStorages_StorageInfos_StorageInfoId",
                        column: x => x.StorageInfoId,
                        principalTable: "StorageInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_IsPrimary_IsAdminAccount",
                table: "BankAccounts",
                columns: new[] { "IsPrimary", "IsAdminAccount" });

            migrationBuilder.CreateIndex(
                name: "IX_ExtendStorages_StorageInfoId",
                table: "ExtendStorages",
                column: "StorageInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageInfos_UserId",
                table: "StorageInfos",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtendStorages");

            migrationBuilder.DropTable(
                name: "StorageInfos");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_IsPrimary_IsAdminAccount",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PaymentTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "RelatedInformation",
                table: "PaymentTransactions",
                type: "json",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
