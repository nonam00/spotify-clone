using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "users");

            migrationBuilder.DropColumn(
                name: "billing_address",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "payment_method",
                table: "users",
                newName: "avatar_path");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "avatar_path",
                table: "users",
                newName: "payment_method");

            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "billing_address",
                table: "users",
                type: "text",
                nullable: true);
        }
    }
}
