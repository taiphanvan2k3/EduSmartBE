using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PaymentService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableStructure : Migration
    {
        protected void ChangeIdOfBanks(MigrationBuilder migrationBuilder)
        {
            // Xoá cột BankId trong BankAccounts
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_BankId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "BankAccounts");

            // Thêm cột BankId mới trong BankAccounts với type là int và không tự tăng
            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "BankAccounts",
                type: "integer",
                nullable: false,
                comment: "Bank Id");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Banks");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Banks",
                type: "integer",
                nullable: false,
                comment: "Bank Id")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.None);

            // Add primary key constraint
            migrationBuilder.AddPrimaryKey(
                name: "PK_Banks",
                table: "Banks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Banks_BankId",
                table: "BankAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentTransactions");

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "StudentTransactions");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "SwiftCode",
                table: "Banks");

            migrationBuilder.RenameColumn(
                name: "TotalEarnings",
                table: "TeacherEarnings",
                newName: "TotalWithdrawn");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "StudentTransactions",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "Banks",
                newName: "LogoURL");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "WithdrawalRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "WithdrawalRequests",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BelongToTeacherId",
                table: "StudentTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "The teacher who the student paid to");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StudentTransactions",
                type: "character varying(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "StudentTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "StudentTransactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "StudentTransactions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "StudentTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ResolvedAt",
                table: "StudentTransactions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Banks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Banks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LogoURL",
                table: "Banks",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Banks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            // Xoá khoá ngoại liên kết của BankAccounts với Banks
            ChangeIdOfBanks(migrationBuilder);

            migrationBuilder.AddColumn<string>(
                name: "Bin",
                table: "Banks",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                comment: "Bank Identification Number");

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Banks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "BankAccounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "BankAccounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "BankAccounts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "BankAccounts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_UserId",
                table: "WithdrawalRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WithdrawalRequests_TeacherEarnings_UserId",
                table: "WithdrawalRequests",
                column: "UserId",
                principalTable: "TeacherEarnings",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithdrawalRequests_TeacherEarnings_UserId",
                table: "WithdrawalRequests");

            migrationBuilder.DropIndex(
                name: "IX_WithdrawalRequests_UserId",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "BelongToTeacherId",
                table: "StudentTransactions");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "StudentTransactions");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "StudentTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StudentTransactions");

            migrationBuilder.DropColumn(
                name: "Error",
                table: "StudentTransactions");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "StudentTransactions");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "StudentTransactions");

            migrationBuilder.DropColumn(
                name: "Bin",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Banks");

            migrationBuilder.RenameColumn(
                name: "TotalWithdrawn",
                table: "TeacherEarnings",
                newName: "TotalEarnings");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "StudentTransactions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "LogoURL",
                table: "Banks",
                newName: "LogoUrl");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StudentTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "StudentTransactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Banks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Banks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                table: "Banks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Banks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Banks",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Banks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwiftCode",
                table: "Banks",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "BankAccounts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "BankAccounts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankId",
                table: "BankAccounts",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                table: "BankAccounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "BankAccounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
