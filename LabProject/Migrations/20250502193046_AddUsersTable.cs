using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabProject.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
    
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Username", "Password", "Role", "IsActive", "CreatedAt" },
                values: new object[] { 1, "admin", "admin123", "Administrator", true, new DateTime(2025, 4, 18) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Username", "Password", "Role", "IsActive", "CreatedAt" },
                values: new object[] { 2, "user", "user123", "User", true, new DateTime(2025, 4, 18) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
