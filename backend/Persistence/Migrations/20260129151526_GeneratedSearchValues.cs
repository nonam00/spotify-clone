using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GeneratedSearchValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_songs_author",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_songs_title",
                table: "songs");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gin", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,");

            migrationBuilder.AddColumn<string>(
                name: "author_lower",
                table: "songs",
                type: "text",
                nullable: true,
                computedColumnSql: "lower(f_unaccent(trim(\"author\")))",
                stored: true);

            migrationBuilder.AddColumn<string>(
                name: "title_lower",
                table: "songs",
                type: "text",
                nullable: true,
                computedColumnSql: "lower(f_unaccent(trim(\"title\")))",
                stored: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_songs_author_lower",
                table: "songs");

            migrationBuilder.DropIndex(
                name: "ix_songs_title_lower",
                table: "songs");

            migrationBuilder.DropColumn(
                name: "author_lower",
                table: "songs");

            migrationBuilder.DropColumn(
                name: "title_lower",
                table: "songs");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:btree_gin", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:unaccent", ",,");

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
    }
}
