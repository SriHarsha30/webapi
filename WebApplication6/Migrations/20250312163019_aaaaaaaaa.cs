using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class aaaaaaaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Owner_Id",
                table: "Properties",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Leases1",
                columns: table => new
                {
                    LeaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Property_Id = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tenant_Signature = table.Column<bool>(type: "bit", nullable: false),
                    Owner_Signature = table.Column<bool>(type: "bit", nullable: false),
                    Lease_status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leases1", x => x.LeaseId);
                    table.ForeignKey(
                        name: "FK_Leases1_Properties_Property_Id",
                        column: x => x.Property_Id,
                        principalTable: "Properties",
                        principalColumn: "Property_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Leases1_Registrationss_ID",
                        column: x => x.ID,
                        principalTable: "Registrationss",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "notifications1",
                columns: table => new
                {
                    Notification_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sendersId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    receiversId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    notification_Descpirtion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications1", x => x.Notification_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Owner_Id",
                table: "Properties",
                column: "Owner_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Leases1_ID",
                table: "Leases1",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_Leases1_Property_Id",
                table: "Leases1",
                column: "Property_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Registrationss_Owner_Id",
                table: "Properties",
                column: "Owner_Id",
                principalTable: "Registrationss",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Registrationss_Owner_Id",
                table: "Properties");

            migrationBuilder.DropTable(
                name: "Leases1");

            migrationBuilder.DropTable(
                name: "notifications1");

            migrationBuilder.DropIndex(
                name: "IX_Properties_Owner_Id",
                table: "Properties");

            migrationBuilder.AlterColumn<string>(
                name: "Owner_Id",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
