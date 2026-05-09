using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IsPublishedIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_songs_author_lower",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_songs_title_lower",
                table: "songs");

            migrationBuilder.CreateIndex(
                name: "ix_songs_author_lower",
                table: "songs",
                column: "author_lower",
                filter: "\"is_published\" = true")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_songs_is_published",
                table: "songs",
                column: "is_published");

            migrationBuilder.CreateIndex(
                name: "ix_songs_title_lower",
                table: "songs",
                column: "title_lower",
                filter: "\"is_published\" = true")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_songs_author_lower",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_songs_is_published",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_songs_title_lower",
                table: "songs");

            migrationBuilder.CreateIndex(
                name: "ix_songs_author_lower",
                table: "songs",
                column: "author_lower")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_songs_title_lower",
                table: "songs",
                column: "title_lower")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }
    }
}
