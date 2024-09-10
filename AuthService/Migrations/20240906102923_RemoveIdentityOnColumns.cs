using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdentityOnColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Functions_Screens_ScreenId",
                table: "Functions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Functions_FunctionId",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Screens",
                table: "Screens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_FunctionId",
                table: "Permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Functions",
                table: "Functions");

            migrationBuilder.DropIndex(
                name: "IX_Functions_ScreenId",
                table: "Functions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Screens");

            migrationBuilder.RenameColumn(
                name: "FunctionName",
                table: "Functions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Functions",
                newName: "Code");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Screens",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Functions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Functions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Functions");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Functions",
                newName: "FunctionName");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Functions",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Screens",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Screens",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Functions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Screens",
                table: "Screens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                columns: new[] { "RoleId", "FunctionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Functions",
                table: "Functions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_FunctionId",
                table: "Permissions",
                column: "FunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Functions_ScreenId",
                table: "Functions",
                column: "ScreenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Functions_Screens_ScreenId",
                table: "Functions",
                column: "ScreenId",
                principalTable: "Screens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Functions_FunctionId",
                table: "Permissions",
                column: "FunctionId",
                principalTable: "Functions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
