using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GinIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_liked_songs_user_id_song_id",
                table: "liked_songs");

            migrationBuilder.CreateIndex(
                name: "ix_songs_author",
                table: "songs",
                column: "author")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "ix_songs_title",
                table: "songs",
                column: "title")
                .Annotation("Npgsql:IndexMethod", "gin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_songs_author",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_songs_title",
                table: "songs");

            migrationBuilder.CreateIndex(
                name: "ix_liked_songs_user_id_song_id",
                table: "liked_songs",
                columns: new[] { "user_id", "song_id" },
                unique: true);
        }
    }
}
