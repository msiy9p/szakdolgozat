using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Libellus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CommentReplyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "text",
                schema: "libellus_social",
                table: "comment",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000)
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "modified_on_utc",
                schema: "libellus_social",
                table: "comment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "created_on_utc",
                schema: "libellus_social",
                table: "comment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AddColumn<Guid>(
                name: "replied_to_id",
                schema: "libellus_social",
                table: "comment",
                type: "uuid",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.CreateIndex(
                name: "ix_comment_replied_to_id",
                schema: "libellus_social",
                table: "comment",
                column: "replied_to_id");

            migrationBuilder.AddForeignKey(
                name: "fk_comment_comment_replied_to_id",
                schema: "libellus_social",
                table: "comment",
                column: "replied_to_id",
                principalSchema: "libellus_social",
                principalTable: "comment",
                principalColumn: "comment_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comment_comment_replied_to_id",
                schema: "libellus_social",
                table: "comment");

            migrationBuilder.DropIndex(
                name: "ix_comment_replied_to_id",
                schema: "libellus_social",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "replied_to_id",
                schema: "libellus_social",
                table: "comment");

            migrationBuilder.AlterColumn<string>(
                name: "text",
                schema: "libellus_social",
                table: "comment",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10000)",
                oldMaxLength: 10000)
                .Annotation("Relational:ColumnOrder", 6)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "modified_on_utc",
                schema: "libellus_social",
                table: "comment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 5)
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<ZonedDateTime>(
                name: "created_on_utc",
                schema: "libellus_social",
                table: "comment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(ZonedDateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 4)
                .OldAnnotation("Relational:ColumnOrder", 5);
        }
    }
}
