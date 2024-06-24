using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Playlists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    billing_address = table.Column<string>(type: "text", nullable: true),
                    payment_method = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "playlists",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_path = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 865, DateTimeKind.Utc).AddTicks(7809))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlists", x => x.id);
                    table.ForeignKey(
                        name: "fk_playlists_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "songs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 862, DateTimeKind.Utc).AddTicks(3660)),
                    title = table.Column<string>(type: "text", nullable: false),
                    song_path = table.Column<string>(type: "text", nullable: false),
                    image_path = table.Column<string>(type: "text", nullable: false),
                    author = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_songs", x => x.id);
                    table.ForeignKey(
                        name: "fk_songs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "liked_songs",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    song_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 863, DateTimeKind.Utc).AddTicks(1798))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_liked_songs", x => new { x.user_id, x.song_id });
                    table.ForeignKey(
                        name: "fk_liked_songs_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_liked_songs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlist_songs",
                columns: table => new
                {
                    playlist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    song_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2024, 6, 24, 4, 30, 3, 865, DateTimeKind.Utc).AddTicks(8777))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_playlist_songs", x => new { x.playlist_id, x.song_id });
                    table.ForeignKey(
                        name: "fk_playlist_songs_playlists_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_songs_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_liked_songs_song_id",
                table: "liked_songs",
                column: "song_id");

            migrationBuilder.CreateIndex(
                name: "ix_liked_songs_user_id_song_id",
                table: "liked_songs",
                columns: new[] { "user_id", "song_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_playlist_songs_song_id",
                table: "playlist_songs",
                column: "song_id");

            migrationBuilder.CreateIndex(
                name: "ix_playlists_user_id",
                table: "playlists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_songs_user_id",
                table: "songs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "liked_songs");

            migrationBuilder.DropTable(
                name: "playlist_songs");

            migrationBuilder.DropTable(
                name: "playlists");

            migrationBuilder.DropTable(
                name: "songs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
