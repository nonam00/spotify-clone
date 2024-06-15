using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _15_06_24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_liked_songs_users_user_id",
                table: "liked_songs");

            migrationBuilder.DropForeignKey(
                name: "fk_songs_users_user_id",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_users_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_songs_id",
                table: "songs");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "songs",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "songs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_DATE",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "liked_songs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_DATE",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddForeignKey(
                name: "fk_liked_songs_users_user_id",
                table: "liked_songs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_songs_users_user_id",
                table: "songs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_liked_songs_users_user_id",
                table: "liked_songs");

            migrationBuilder.DropForeignKey(
                name: "fk_songs_users_user_id",
                table: "songs");

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "songs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "songs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_DATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "liked_songs",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_DATE");

            migrationBuilder.CreateIndex(
                name: "ix_users_id",
                table: "users",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_songs_id",
                table: "songs",
                column: "id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_liked_songs_users_user_id",
                table: "liked_songs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_songs_users_user_id",
                table: "songs",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
