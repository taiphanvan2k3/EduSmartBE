using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Migrations
{
    /// <inheritdoc />
    public partial class ModifyPaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_BankAccounts_BankAccountId",
                table: "WithdrawalRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_TeacherEarnings_UserId",
                table: "WithdrawalRequests");

            migrationBuilder.DropTable(
                name: "StudentTransactions");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdminAccount",
                table: "BankAccounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Error = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PaymentMethod = table.Column<string>(type: "text", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    OrderStatus = table.Column<string>(type: "text", nullable: false),
                    RelatedInformation = table.Column<string>(type: "json", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_BankAccounts_BankAccountId",
                table: "WithdrawalRequests",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_TeacherEarnings_UserId",
                table: "WithdrawalRequests",
                column: "UserId",
                principalTable: "TeacherEarnings",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_BankAccounts_BankAccountId",
                table: "WithdrawalRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_TeacherEarnings_UserId",
                table: "WithdrawalRequests");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "IsAdminAccount",
                table: "BankAccounts");

            migrationBuilder.CreateTable(
                name: "StudentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    BelongToTeacherId = table.Column<int>(type: "integer", nullable: false, comment: "The teacher who the student paid to"),
                    Code = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: true),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Error = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    OrderStatus = table.Column<string>(type: "text", nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StudentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTransactions", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_BankAccounts_BankAccountId",
                table: "WithdrawalRequests",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_TeacherEarnings_UserId",
                table: "WithdrawalRequests",
                column: "UserId",
                principalTable: "TeacherEarnings",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
