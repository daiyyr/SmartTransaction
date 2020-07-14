using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureIoTPortal.Migrations
{
    public partial class addDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    iot_id = table.Column<string>(nullable: true),
                    primary_key = table.Column<string>(nullable: true),
                    conneciton_string = table.Column<string>(nullable: true),
                    connection_state = table.Column<string>(nullable: true),
                    last_conection_state_update_time = table.Column<DateTime>(nullable: true),
                    last_activity_time = table.Column<DateTime>(nullable: true),
                    stat = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
