using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "songs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 862, DateTimeKind.Utc).AddTicks(3660));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "playlists",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 865, DateTimeKind.Utc).AddTicks(7809));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "playlist_songs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 865, DateTimeKind.Utc).AddTicks(8777));

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "liked_songs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 863, DateTimeKind.Utc).AddTicks(1798));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "songs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 862, DateTimeKind.Utc).AddTicks(3660),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "playlists",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 865, DateTimeKind.Utc).AddTicks(7809),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "playlist_songs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 865, DateTimeKind.Utc).AddTicks(8777),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "liked_songs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 863, DateTimeKind.Utc).AddTicks(1798),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
