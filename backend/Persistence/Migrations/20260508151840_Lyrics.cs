using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Lyrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "contains_explicit_content",
                table: "songs",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.CreateTable(
                name: "lyrics_segments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    song_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start = table.Column<double>(type: "double precision", nullable: false),
                    end = table.Column<double>(type: "double precision", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    normalized_text = table.Column<string>(type: "text", nullable: true, computedColumnSql: "lower(f_unaccent(trim(\"text\")))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lyrics_segments", x => x.id);
                    table.ForeignKey(
                        name: "fk_lyrics_segments_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_lyrics_segments_normalized_text",
                table: "lyrics_segments",
                column: "normalized_text")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_lyrics_segments_song_id",
                table: "lyrics_segments",
                column: "song_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lyrics_segments");

            migrationBuilder.AlterColumn<bool>(
                name: "contains_explicit_content",
                table: "songs",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
