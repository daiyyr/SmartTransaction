using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureIoTPortal.Migrations
{
    public partial class updateevent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "event_type",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "queue_number",
                table: "Events",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "queue_number",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "event_type",
                table: "Events",
                nullable: true);
        }
    }
}
