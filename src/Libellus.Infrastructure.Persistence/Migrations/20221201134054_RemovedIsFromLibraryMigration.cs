using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Libellus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedIsFromLibraryMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_from_library",
                schema: "libellus_social",
                table: "book_edition");

            migrationBuilder.AlterColumn<string>(
                name: "title_normalized",
                schema: "libellus_social",
                table: "book_edition",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250)
                .Annotation("Relational:ColumnOrder", 19)
                .OldAnnotation("Relational:ColumnOrder", 20);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "libellus_social",
                table: "book_edition",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250)
                .Annotation("Relational:ColumnOrder", 18)
                .OldAnnotation("Relational:ColumnOrder", 19);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "modified_on_utc",
                schema: "libellus_social",
                table: "book_edition",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 17)
                .OldAnnotation("Relational:ColumnOrder", 18);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "libellus_social",
                table: "book_edition",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 20)
                .OldAnnotation("Relational:ColumnOrder", 21);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "created_on_utc",
                schema: "libellus_social",
                table: "book_edition",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 16)
                .OldAnnotation("Relational:ColumnOrder", 17);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "title_normalized",
                schema: "libellus_social",
                table: "book_edition",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250)
                .Annotation("Relational:ColumnOrder", 20)
                .OldAnnotation("Relational:ColumnOrder", 19);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "libellus_social",
                table: "book_edition",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250)
                .Annotation("Relational:ColumnOrder", 19)
                .OldAnnotation("Relational:ColumnOrder", 18);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "modified_on_utc",
                schema: "libellus_social",
                table: "book_edition",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 18)
                .OldAnnotation("Relational:ColumnOrder", 17);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "libellus_social",
                table: "book_edition",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 21)
                .OldAnnotation("Relational:ColumnOrder", 20);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "created_on_utc",
                schema: "libellus_social",
                table: "book_edition",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 17)
                .OldAnnotation("Relational:ColumnOrder", 16);

            migrationBuilder.AddColumn<bool>(
                name: "is_from_library",
                schema: "libellus_social",
                table: "book_edition",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 16);
        }
    }
}
