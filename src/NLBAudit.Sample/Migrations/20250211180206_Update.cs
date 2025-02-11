using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NLBAudit.Sample.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomData",
                table: "AuditLogs");

            migrationBuilder.AddColumn<string>(
                name: "HttpMethod",
                table: "AuditLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "AuditLogs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HttpMethod",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "AuditLogs");

            migrationBuilder.AddColumn<string>(
                name: "CustomData",
                table: "AuditLogs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
