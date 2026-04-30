using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Photography.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlbumSeoMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverAltText",
                table: "albums",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoDescription",
                table: "albums",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeoTitle",
                table: "albums",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "albums",
                type: "character varying(96)",
                maxLength: 96,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE albums
                SET "Slug" = lower(
                    concat(
                        left(
                            coalesce(nullif(trim(both '-' from regexp_replace("Title", '[^a-zA-Z0-9]+', '-', 'g')), ''), 'album'),
                            87
                        ),
                        '-',
                        left(replace("Id"::text, '-', ''), 8)
                    )
                )
                WHERE "Slug" = '';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_albums_Slug",
                table: "albums",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_albums_Slug",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "CoverAltText",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "SeoDescription",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "SeoTitle",
                table: "albums");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "albums");
        }
    }
}
