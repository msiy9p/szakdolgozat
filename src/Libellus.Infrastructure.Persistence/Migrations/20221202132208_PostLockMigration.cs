using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Libellus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PostLockMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locked_post",
                schema: "libellus_social",
                columns: table => new
                {
                    postid = table.Column<Guid>(name: "post_id", type: "uuid", nullable: false),
                    lockcreatorid = table.Column<Guid>(name: "lock_creator_id", type: "uuid", nullable: true),
                    lockreason = table.Column<string>(name: "lock_reason", type: "character varying(10000)", maxLength: 10000, nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locked_post", x => x.postid);
                    table.ForeignKey(
                        name: "fk_locked_post_asp_net_users_lock_creator_id",
                        column: x => x.lockcreatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_locked_post_posts_post_id",
                        column: x => x.postid,
                        principalSchema: "libellus_social",
                        principalTable: "post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_locked_post_lock_creator_id",
                schema: "libellus_social",
                table: "locked_post",
                column: "lock_creator_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "locked_post",
                schema: "libellus_social");
        }
    }
}
