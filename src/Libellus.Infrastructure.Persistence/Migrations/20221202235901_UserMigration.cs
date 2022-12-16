using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libellus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_author_asp_net_users_user_id",
                schema: "libellus_social",
                table: "author");

            migrationBuilder.DropForeignKey(
                name: "fk_book_asp_net_users_user_id",
                schema: "libellus_social",
                table: "book");

            migrationBuilder.DropForeignKey(
                name: "fk_book_edition_asp_net_users_user_id",
                schema: "libellus_social",
                table: "book_edition");

            migrationBuilder.DropForeignKey(
                name: "fk_format_asp_net_users_user_id",
                schema: "libellus_social",
                table: "format");

            migrationBuilder.DropForeignKey(
                name: "fk_genre_asp_net_users_user_id",
                schema: "libellus_social",
                table: "genre");

            migrationBuilder.DropForeignKey(
                name: "fk_language_asp_net_users_user_id",
                schema: "libellus_social",
                table: "language");

            migrationBuilder.DropForeignKey(
                name: "fk_literature_form_asp_net_users_user_id",
                schema: "libellus_social",
                table: "literature_form");

            migrationBuilder.DropForeignKey(
                name: "fk_note_asp_net_users_user_id",
                schema: "libellus_social",
                table: "note");

            migrationBuilder.DropForeignKey(
                name: "fk_publisher_asp_net_users_user_id",
                schema: "libellus_social",
                table: "publisher");

            migrationBuilder.DropForeignKey(
                name: "fk_reading_asp_net_users_user_id",
                schema: "libellus_social",
                table: "reading");

            migrationBuilder.DropForeignKey(
                name: "fk_series_asp_net_users_user_id",
                schema: "libellus_social",
                table: "series");

            migrationBuilder.DropForeignKey(
                name: "fk_shelf_asp_net_users_user_id",
                schema: "libellus_social",
                table: "shelf");

            migrationBuilder.DropForeignKey(
                name: "fk_tag_asp_net_users_user_id",
                schema: "libellus_social",
                table: "tag");

            migrationBuilder.DropForeignKey(
                name: "fk_warning_tag_asp_net_users_user_id",
                schema: "libellus_social",
                table: "warning_tag");

            migrationBuilder.AddForeignKey(
                name: "fk_author_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "author",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_book_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "book",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_book_edition_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "book_edition",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_format_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "format",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_genre_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "genre",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_language_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "language",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_literature_form_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "literature_form",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_note_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "note",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_publisher_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "publisher",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_reading_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "reading",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_series_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "series",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_shelf_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "shelf",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_tag_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "tag",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_warning_tag_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "warning_tag",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_author_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "author");

            migrationBuilder.DropForeignKey(
                name: "fk_book_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "book");

            migrationBuilder.DropForeignKey(
                name: "fk_book_edition_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "book_edition");

            migrationBuilder.DropForeignKey(
                name: "fk_format_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "format");

            migrationBuilder.DropForeignKey(
                name: "fk_genre_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "genre");

            migrationBuilder.DropForeignKey(
                name: "fk_language_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "language");

            migrationBuilder.DropForeignKey(
                name: "fk_literature_form_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "literature_form");

            migrationBuilder.DropForeignKey(
                name: "fk_note_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "note");

            migrationBuilder.DropForeignKey(
                name: "fk_publisher_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "publisher");

            migrationBuilder.DropForeignKey(
                name: "fk_reading_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "reading");

            migrationBuilder.DropForeignKey(
                name: "fk_series_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "series");

            migrationBuilder.DropForeignKey(
                name: "fk_shelf_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "shelf");

            migrationBuilder.DropForeignKey(
                name: "fk_tag_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "tag");

            migrationBuilder.DropForeignKey(
                name: "fk_warning_tag_asp_net_users_application_user_id",
                schema: "libellus_social",
                table: "warning_tag");

            migrationBuilder.AddForeignKey(
                name: "fk_author_asp_net_users_user_id",
                schema: "libellus_social",
                table: "author",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_book_asp_net_users_user_id",
                schema: "libellus_social",
                table: "book",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_book_edition_asp_net_users_user_id",
                schema: "libellus_social",
                table: "book_edition",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_format_asp_net_users_user_id",
                schema: "libellus_social",
                table: "format",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_genre_asp_net_users_user_id",
                schema: "libellus_social",
                table: "genre",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_language_asp_net_users_user_id",
                schema: "libellus_social",
                table: "language",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_literature_form_asp_net_users_user_id",
                schema: "libellus_social",
                table: "literature_form",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_note_asp_net_users_user_id",
                schema: "libellus_social",
                table: "note",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_publisher_asp_net_users_user_id",
                schema: "libellus_social",
                table: "publisher",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_reading_asp_net_users_user_id",
                schema: "libellus_social",
                table: "reading",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_series_asp_net_users_user_id",
                schema: "libellus_social",
                table: "series",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_shelf_asp_net_users_user_id",
                schema: "libellus_social",
                table: "shelf",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_tag_asp_net_users_user_id",
                schema: "libellus_social",
                table: "tag",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_warning_tag_asp_net_users_user_id",
                schema: "libellus_social",
                table: "warning_tag",
                column: "creator_id",
                principalSchema: "libellus_security",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
