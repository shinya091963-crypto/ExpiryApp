using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpiryApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexOnDeviceAssetTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AssetTag",
                table: "Devices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_AssetTag",
                table: "Devices",
                column: "AssetTag",
                unique: true,
                filter: "[AssetTag] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Devices_AssetTag",
                table: "Devices");

            migrationBuilder.AlterColumn<string>(
                name: "AssetTag",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
