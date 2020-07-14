using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureIoTPortal.Migrations
{
    public partial class updatedevicestate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "stat",
                table: "Devices",
                newName: "state");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "state",
                table: "Devices",
                newName: "stat");
        }
    }
}
