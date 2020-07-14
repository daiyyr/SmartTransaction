using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureIoTPortal.Migrations
{
    public partial class updateevent2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "queue_number",
                table: "Events",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "queue_number",
                table: "Events",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
