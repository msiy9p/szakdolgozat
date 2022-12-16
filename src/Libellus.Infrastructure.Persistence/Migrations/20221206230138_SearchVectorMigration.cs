using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Libellus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SearchVectorMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "warning_tag",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "warning_tag",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_warning_tag_search_vector_simple",
                schema: "libellus_social",
                table: "warning_tag",
                newName: "ix_warning_tag_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_warning_tag_search_vector_custom",
                schema: "libellus_social",
                table: "warning_tag",
                newName: "ix_warning_tag_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "tag",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "tag",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_tag_search_vector_simple",
                schema: "libellus_social",
                table: "tag",
                newName: "ix_tag_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_tag_search_vector_custom",
                schema: "libellus_social",
                table: "tag",
                newName: "ix_tag_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "shelf",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "shelf",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_shelf_search_vector_simple",
                schema: "libellus_social",
                table: "shelf",
                newName: "ix_shelf_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_shelf_search_vector_custom",
                schema: "libellus_social",
                table: "shelf",
                newName: "ix_shelf_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "series",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "series",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_series_search_vector_simple",
                schema: "libellus_social",
                table: "series",
                newName: "ix_series_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_series_search_vector_custom",
                schema: "libellus_social",
                table: "series",
                newName: "ix_series_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "publisher",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "publisher",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_publisher_search_vector_simple",
                schema: "libellus_social",
                table: "publisher",
                newName: "ix_publisher_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_publisher_search_vector_custom",
                schema: "libellus_social",
                table: "publisher",
                newName: "ix_publisher_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "post",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "post",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_post_search_vector_simple",
                schema: "libellus_social",
                table: "post",
                newName: "ix_post_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_post_search_vector_custom",
                schema: "libellus_social",
                table: "post",
                newName: "ix_post_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "literature_form",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "literature_form",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_literature_form_search_vector_simple",
                schema: "libellus_social",
                table: "literature_form",
                newName: "ix_literature_form_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_literature_form_search_vector_custom",
                schema: "libellus_social",
                table: "literature_form",
                newName: "ix_literature_form_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "language",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "language",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_language_search_vector_simple",
                schema: "libellus_social",
                table: "language",
                newName: "ix_language_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_language_search_vector_custom",
                schema: "libellus_social",
                table: "language",
                newName: "ix_language_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "label",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "label",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_label_search_vector_simple",
                schema: "libellus_social",
                table: "label",
                newName: "ix_label_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_label_search_vector_custom",
                schema: "libellus_social",
                table: "label",
                newName: "ix_label_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "group",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "group",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_group_search_vector_simple",
                schema: "libellus_social",
                table: "group",
                newName: "ix_group_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_group_search_vector_custom",
                schema: "libellus_social",
                table: "group",
                newName: "ix_group_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "genre",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "genre",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_genre_search_vector_simple",
                schema: "libellus_social",
                table: "genre",
                newName: "ix_genre_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_genre_search_vector_custom",
                schema: "libellus_social",
                table: "genre",
                newName: "ix_genre_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "format",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "format",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_format_search_vector_simple",
                schema: "libellus_social",
                table: "format",
                newName: "ix_format_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_format_search_vector_custom",
                schema: "libellus_social",
                table: "format",
                newName: "ix_format_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "book_edition",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "book_edition",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_book_edition_search_vector_simple",
                schema: "libellus_social",
                table: "book_edition",
                newName: "ix_book_edition_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_book_edition_search_vector_custom",
                schema: "libellus_social",
                table: "book_edition",
                newName: "ix_book_edition_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "book",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "book",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_book_search_vector_simple",
                schema: "libellus_social",
                table: "book",
                newName: "ix_book_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_book_search_vector_custom",
                schema: "libellus_social",
                table: "book",
                newName: "ix_book_search_vector_one");

            migrationBuilder.RenameColumn(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "author",
                newName: "search_vector_two");

            migrationBuilder.RenameColumn(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "author",
                newName: "search_vector_one");

            migrationBuilder.RenameIndex(
                name: "ix_author_search_vector_simple",
                schema: "libellus_social",
                table: "author",
                newName: "ix_author_search_vector_two");

            migrationBuilder.RenameIndex(
                name: "ix_author_search_vector_custom",
                schema: "libellus_social",
                table: "author",
                newName: "ix_author_search_vector_one");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "warning_tag",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "warning_tag",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "tag",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "tag",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "shelf",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "shelf",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "series",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "series",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "publisher",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "publisher",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "post",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "text" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "text" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "post",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "text" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "text" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "literature_form",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "literature_form",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "language",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "language",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "label",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "label",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "group",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "group",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "genre",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "genre",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "format",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "format",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "book_edition",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "book_edition",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "book",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "book",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "author",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "author",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "warning_tag",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "warning_tag",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_warning_tag_search_vector_two",
                schema: "libellus_social",
                table: "warning_tag",
                newName: "ix_warning_tag_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_warning_tag_search_vector_one",
                schema: "libellus_social",
                table: "warning_tag",
                newName: "ix_warning_tag_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "tag",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "tag",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_tag_search_vector_two",
                schema: "libellus_social",
                table: "tag",
                newName: "ix_tag_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_tag_search_vector_one",
                schema: "libellus_social",
                table: "tag",
                newName: "ix_tag_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "shelf",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "shelf",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_shelf_search_vector_two",
                schema: "libellus_social",
                table: "shelf",
                newName: "ix_shelf_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_shelf_search_vector_one",
                schema: "libellus_social",
                table: "shelf",
                newName: "ix_shelf_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "series",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "series",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_series_search_vector_two",
                schema: "libellus_social",
                table: "series",
                newName: "ix_series_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_series_search_vector_one",
                schema: "libellus_social",
                table: "series",
                newName: "ix_series_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "publisher",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "publisher",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_publisher_search_vector_two",
                schema: "libellus_social",
                table: "publisher",
                newName: "ix_publisher_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_publisher_search_vector_one",
                schema: "libellus_social",
                table: "publisher",
                newName: "ix_publisher_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "post",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "post",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_post_search_vector_two",
                schema: "libellus_social",
                table: "post",
                newName: "ix_post_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_post_search_vector_one",
                schema: "libellus_social",
                table: "post",
                newName: "ix_post_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "literature_form",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "literature_form",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_literature_form_search_vector_two",
                schema: "libellus_social",
                table: "literature_form",
                newName: "ix_literature_form_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_literature_form_search_vector_one",
                schema: "libellus_social",
                table: "literature_form",
                newName: "ix_literature_form_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "language",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "language",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_language_search_vector_two",
                schema: "libellus_social",
                table: "language",
                newName: "ix_language_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_language_search_vector_one",
                schema: "libellus_social",
                table: "language",
                newName: "ix_language_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "label",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "label",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_label_search_vector_two",
                schema: "libellus_social",
                table: "label",
                newName: "ix_label_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_label_search_vector_one",
                schema: "libellus_social",
                table: "label",
                newName: "ix_label_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "group",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "group",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_group_search_vector_two",
                schema: "libellus_social",
                table: "group",
                newName: "ix_group_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_group_search_vector_one",
                schema: "libellus_social",
                table: "group",
                newName: "ix_group_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "genre",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "genre",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_genre_search_vector_two",
                schema: "libellus_social",
                table: "genre",
                newName: "ix_genre_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_genre_search_vector_one",
                schema: "libellus_social",
                table: "genre",
                newName: "ix_genre_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "format",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "format",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_format_search_vector_two",
                schema: "libellus_social",
                table: "format",
                newName: "ix_format_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_format_search_vector_one",
                schema: "libellus_social",
                table: "format",
                newName: "ix_format_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "book_edition",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "book_edition",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_book_edition_search_vector_two",
                schema: "libellus_social",
                table: "book_edition",
                newName: "ix_book_edition_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_book_edition_search_vector_one",
                schema: "libellus_social",
                table: "book_edition",
                newName: "ix_book_edition_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "book",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "book",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_book_search_vector_two",
                schema: "libellus_social",
                table: "book",
                newName: "ix_book_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_book_search_vector_one",
                schema: "libellus_social",
                table: "book",
                newName: "ix_book_search_vector_custom");

            migrationBuilder.RenameColumn(
                name: "search_vector_two",
                schema: "libellus_social",
                table: "author",
                newName: "search_vector_simple");

            migrationBuilder.RenameColumn(
                name: "search_vector_one",
                schema: "libellus_social",
                table: "author",
                newName: "search_vector_custom");

            migrationBuilder.RenameIndex(
                name: "ix_author_search_vector_two",
                schema: "libellus_social",
                table: "author",
                newName: "ix_author_search_vector_simple");

            migrationBuilder.RenameIndex(
                name: "ix_author_search_vector_one",
                schema: "libellus_social",
                table: "author",
                newName: "ix_author_search_vector_custom");

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "warning_tag",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "warning_tag",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "tag",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "tag",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "shelf",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "shelf",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "series",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "series",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "publisher",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "publisher",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "post",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "text" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "text" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "post",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "text" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "text" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "literature_form",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "literature_form",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "language",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "language",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "label",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "label",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "group",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "group",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "genre",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "genre",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "format",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "format",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "book_edition",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "book_edition",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "book",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "book",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "title", "description" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_simple",
                schema: "libellus_social",
                table: "author",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "english")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });

            migrationBuilder.AlterColumn<NpgsqlTsVector>(
                name: "search_vector_custom",
                schema: "libellus_social",
                table: "author",
                type: "tsvector",
                nullable: false,
                oldClrType: typeof(NpgsqlTsVector),
                oldType: "tsvector")
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                .OldAnnotation("Npgsql:TsVectorConfig", "simple")
                .OldAnnotation("Npgsql:TsVectorProperties", new[] { "name" });
        }
    }
}
