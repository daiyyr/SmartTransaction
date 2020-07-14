using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureIoTPortal.Migrations
{
    public partial class updatedevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "chart_x",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chart_y",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "message_count",
                table: "Devices",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "chart_x",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "chart_y",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "message_count",
                table: "Devices");
        }
    }
}
