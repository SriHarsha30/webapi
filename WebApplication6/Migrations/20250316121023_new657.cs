using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class new657 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Tenant_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tenant_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tenant_Phonenumber = table.Column<long>(type: "bigint", nullable: false),
                    leased_property_id = table.Column<int>(type: "int", nullable: false),
                    startTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Histories", x => x.Tenant_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Histories");
        }
    }
}
