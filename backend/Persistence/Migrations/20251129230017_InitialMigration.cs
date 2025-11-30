using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "moderators",
                columns: new[] { "id", "created_at", "email", "full_name", "is_active", "password_hash", "can_manage_content", "can_manage_moderators", "can_manage_users", "can_view_reports" },
                values: new object[] { new Guid("12dbc10a-a7a9-47a3-9a1b-513aae383f1f"), new DateTime(2023, 4, 14, 21, 0, 0, 0, DateTimeKind.Utc), "admin@admin.com", "Admin", true, "$2a$11$1Aw9/S0I8DnN7VDPmP4EuOl3qREfsNBFwfc8JOIqhldNYDYMBARLC", true, true, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "moderators",
                keyColumn: "id",
                keyValue: new Guid("12dbc10a-a7a9-47a3-9a1b-513aae383f1f"));
        }
    }
}
