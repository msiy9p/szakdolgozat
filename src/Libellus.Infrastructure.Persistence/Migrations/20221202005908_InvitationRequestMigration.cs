using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Libellus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InvitationRequestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invitation_request",
                schema: "libellus_social",
                columns: table => new
                {
                    invitationrequestid = table.Column<Guid>(name: "invitation_request_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    requesterid = table.Column<Guid>(name: "requester_id", type: "uuid", nullable: false),
                    invitationstatus = table.Column<int>(name: "invitation_status", type: "integer", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitation_request", x => x.invitationrequestid);
                    table.ForeignKey(
                        name: "fk_invitation_request_asp_net_users_requester_id",
                        column: x => x.requesterid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invitation_request_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_request_group_id_invitation_request_id",
                schema: "libellus_social",
                table: "invitation_request",
                columns: new[] { "group_id", "invitation_request_id" });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_request_invitation_request_id_invitation_status",
                schema: "libellus_social",
                table: "invitation_request",
                columns: new[] { "invitation_request_id", "invitation_status" },
                filter: "invitation_status = 1")
                .Annotation("Npgsql:IndexInclude", new[] { "created_on_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_request_requester_id",
                schema: "libellus_social",
                table: "invitation_request",
                column: "requester_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invitation_request",
                schema: "libellus_social");
        }
    }
}
