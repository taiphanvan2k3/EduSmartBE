using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWithdrawalRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorInfo",
                table: "WithdrawalRequests",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "WithdrawalRequests",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorInfo",
                table: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "WithdrawalRequests");
        }
    }
}
