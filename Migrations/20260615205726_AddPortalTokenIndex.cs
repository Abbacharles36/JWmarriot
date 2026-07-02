using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWmarriot.Migrations
{
    /// <inheritdoc />
    public partial class AddPortalTokenIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PortalToken",
                table: "Merchants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_PortalToken",
                table: "Merchants",
                column: "PortalToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Merchants_PortalToken",
                table: "Merchants");

            migrationBuilder.AlterColumn<string>(
                name: "PortalToken",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
