using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace Libellus.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "libellus_security");

            migrationBuilder.EnsureSchema(
                name: "libellus_social");

            migrationBuilder.EnsureSchema(
                name: "libellus_media");

            migrationBuilder.CreateTable(
                name: "asp_net_roles",
                schema: "libellus_security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedname = table.Column<string>(name: "normalized_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrencystamp = table.Column<string>(name: "concurrency_stamp", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_users",
                schema: "libellus_security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profilepictureid = table.Column<Guid>(name: "profile_picture_id", type: "uuid", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    username = table.Column<string>(name: "user_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedusername = table.Column<string>(name: "normalized_user_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedemail = table.Column<string>(name: "normalized_email", type: "character varying(256)", maxLength: 256, nullable: true),
                    emailconfirmed = table.Column<bool>(name: "email_confirmed", type: "boolean", nullable: false),
                    passwordhash = table.Column<string>(name: "password_hash", type: "text", nullable: true),
                    securitystamp = table.Column<string>(name: "security_stamp", type: "text", nullable: true),
                    concurrencystamp = table.Column<string>(name: "concurrency_stamp", type: "text", nullable: true),
                    phonenumber = table.Column<string>(name: "phone_number", type: "text", nullable: true),
                    phonenumberconfirmed = table.Column<bool>(name: "phone_number_confirmed", type: "boolean", nullable: false),
                    twofactorenabled = table.Column<bool>(name: "two_factor_enabled", type: "boolean", nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(name: "lockout_end", type: "timestamp with time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(name: "lockout_enabled", type: "boolean", nullable: false),
                    accessfailedcount = table.Column<int>(name: "access_failed_count", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cover_image_meta_data",
                schema: "libellus_media",
                columns: table => new
                {
                    coverimagemetadataid = table.Column<long>(name: "cover_image_meta_data_id", type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    publicid = table.Column<Guid>(name: "public_id", type: "uuid", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    mimetype = table.Column<string>(name: "mime_type", type: "character varying(256)", maxLength: 256, nullable: false),
                    datasize = table.Column<int>(name: "data_size", type: "integer", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    objectname = table.Column<string>(name: "object_name", type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cover_image_meta_data", x => x.coverimagemetadataid);
                });

            migrationBuilder.CreateTable(
                name: "group",
                schema: "libellus_social",
                columns: table => new
                {
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    groupfriendlyid = table.Column<string>(name: "group_friendly_id", type: "character varying(12)", maxLength: 12, nullable: false),
                    isprivate = table.Column<bool>(name: "is_private", type: "boolean", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group", x => x.groupid);
                    table.UniqueConstraint("ak_group_group_friendly_id", x => x.groupfriendlyid);
                });

            migrationBuilder.CreateTable(
                name: "group_role",
                schema: "libellus_security",
                columns: table => new
                {
                    grouproleid = table.Column<Guid>(name: "group_role_id", type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_role", x => x.grouproleid);
                });

            migrationBuilder.CreateTable(
                name: "profile_picture_meta_data",
                schema: "libellus_media",
                columns: table => new
                {
                    profilepicturemetadataid = table.Column<long>(name: "profile_picture_meta_data_id", type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    publicid = table.Column<Guid>(name: "public_id", type: "uuid", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    mimetype = table.Column<string>(name: "mime_type", type: "character varying(256)", maxLength: 256, nullable: false),
                    datasize = table.Column<int>(name: "data_size", type: "integer", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    objectname = table.Column<string>(name: "object_name", type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profile_picture_meta_data", x => x.profilepicturemetadataid);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_role_claims",
                schema: "libellus_security",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<Guid>(name: "role_id", type: "uuid", nullable: false),
                    claimtype = table.Column<string>(name: "claim_type", type: "text", nullable: true),
                    claimvalue = table.Column<string>(name: "claim_value", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.roleid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_claims",
                schema: "libellus_security",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    claimtype = table.Column<string>(name: "claim_type", type: "text", nullable: true),
                    claimvalue = table.Column<string>(name: "claim_value", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.userid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_logins",
                schema: "libellus_security",
                columns: table => new
                {
                    loginprovider = table.Column<string>(name: "login_provider", type: "text", nullable: false),
                    providerkey = table.Column<string>(name: "provider_key", type: "text", nullable: false),
                    providerdisplayname = table.Column<string>(name: "provider_display_name", type: "text", nullable: true),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.userid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_roles",
                schema: "libellus_security",
                columns: table => new
                {
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    roleid = table.Column<Guid>(name: "role_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.roleid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.userid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asp_net_user_tokens",
                schema: "libellus_security",
                columns: table => new
                {
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    loginprovider = table.Column<string>(name: "login_provider", type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.userid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "author",
                schema: "libellus_social",
                columns: table => new
                {
                    authorid = table.Column<Guid>(name: "author_id", type: "uuid", nullable: false),
                    authorfriendlyid = table.Column<string>(name: "author_friendly_id", type: "character varying(12)", maxLength: 12, nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    coverimageid = table.Column<Guid>(name: "cover_image_id", type: "uuid", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(250)", maxLength: 250, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_author", x => x.authorid);
                    table.UniqueConstraint("ak_author_author_friendly_id", x => x.authorfriendlyid);
                    table.ForeignKey(
                        name: "fk_author_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_author_groups_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "format",
                schema: "libellus_social",
                columns: table => new
                {
                    formatid = table.Column<Guid>(name: "format_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    isdigital = table.Column<bool>(name: "is_digital", type: "boolean", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(50)", maxLength: 50, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_format", x => x.formatid);
                    table.ForeignKey(
                        name: "fk_format_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_format_groups_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "genre",
                schema: "libellus_social",
                columns: table => new
                {
                    genreid = table.Column<Guid>(name: "genre_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    isfiction = table.Column<bool>(name: "is_fiction", type: "boolean", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(50)", maxLength: 50, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genre", x => x.genreid);
                    table.ForeignKey(
                        name: "fk_genre_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_genre_groups_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invitation",
                schema: "libellus_social",
                columns: table => new
                {
                    invitationid = table.Column<Guid>(name: "invitation_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    inviterid = table.Column<Guid>(name: "inviter_id", type: "uuid", nullable: false),
                    inviteeid = table.Column<Guid>(name: "invitee_id", type: "uuid", nullable: false),
                    invitationstatus = table.Column<int>(name: "invitation_status", type: "integer", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitation", x => x.invitationid);
                    table.ForeignKey(
                        name: "fk_invitation_asp_net_users_invitee_id",
                        column: x => x.inviteeid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invitation_asp_net_users_inviter_id",
                        column: x => x.inviterid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invitation_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "label",
                schema: "libellus_social",
                columns: table => new
                {
                    labelid = table.Column<Guid>(name: "label_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(50)", maxLength: 50, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_label", x => x.labelid);
                    table.ForeignKey(
                        name: "fk_label_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "language",
                schema: "libellus_social",
                columns: table => new
                {
                    languageid = table.Column<Guid>(name: "language_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(50)", maxLength: 50, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_language", x => x.languageid);
                    table.ForeignKey(
                        name: "fk_language_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_language_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "literature_form",
                schema: "libellus_social",
                columns: table => new
                {
                    literatureformid = table.Column<Guid>(name: "literature_form_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    scoremultiplier = table.Column<decimal>(name: "score_multiplier", type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(50)", maxLength: 50, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_literature_form", x => x.literatureformid);
                    table.ForeignKey(
                        name: "fk_literature_form_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_literature_form_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "note",
                schema: "libellus_social",
                columns: table => new
                {
                    noteid = table.Column<Guid>(name: "note_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_note", x => x.noteid);
                    table.ForeignKey(
                        name: "fk_note_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_note_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "publisher",
                schema: "libellus_social",
                columns: table => new
                {
                    publisherid = table.Column<Guid>(name: "publisher_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(50)", maxLength: 50, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publisher", x => x.publisherid);
                    table.ForeignKey(
                        name: "fk_publisher_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_publisher_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "series",
                schema: "libellus_social",
                columns: table => new
                {
                    seriesid = table.Column<Guid>(name: "series_id", type: "uuid", nullable: false),
                    seriesfriendlyid = table.Column<string>(name: "series_friendly_id", type: "character varying(12)", maxLength: 12, nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    titlenormalized = table.Column<string>(name: "title_normalized", type: "character varying(250)", maxLength: 250, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "title" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "title" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_series", x => x.seriesid);
                    table.UniqueConstraint("ak_series_series_friendly_id", x => x.seriesfriendlyid);
                    table.ForeignKey(
                        name: "fk_series_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_series_groups_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shelf",
                schema: "libellus_social",
                columns: table => new
                {
                    shelfid = table.Column<Guid>(name: "shelf_id", type: "uuid", nullable: false),
                    shelffriendlyid = table.Column<string>(name: "shelf_friendly_id", type: "character varying(12)", maxLength: 12, nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name", "description" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shelf", x => x.shelfid);
                    table.UniqueConstraint("ak_shelf_shelf_friendly_id", x => x.shelffriendlyid);
                    table.ForeignKey(
                        name: "fk_shelf_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_shelf_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tag",
                schema: "libellus_social",
                columns: table => new
                {
                    tagid = table.Column<Guid>(name: "tag_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(50)", maxLength: 50, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag", x => x.tagid);
                    table.ForeignKey(
                        name: "fk_tag_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_tag_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "warning_tag",
                schema: "libellus_social",
                columns: table => new
                {
                    warningtagid = table.Column<Guid>(name: "warning_tag_id", type: "uuid", nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    namenormalized = table.Column<string>(name: "name_normalized", type: "character varying(50)", maxLength: 50, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "name" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_warning_tag", x => x.warningtagid);
                    table.ForeignKey(
                        name: "fk_warning_tag_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_warning_tag_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "group_user_membership",
                schema: "libellus_social",
                columns: table => new
                {
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    grouproleid = table.Column<Guid>(name: "group_role_id", type: "uuid", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_user_membership", x => new { x.groupid, x.userid });
                    table.ForeignKey(
                        name: "fk_group_user_membership_asp_net_users_user_id",
                        column: x => x.userid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_group_user_membership_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_group_user_membership_group_role_group_role_id",
                        column: x => x.grouproleid,
                        principalSchema: "libellus_security",
                        principalTable: "group_role",
                        principalColumn: "group_role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post",
                schema: "libellus_social",
                columns: table => new
                {
                    postid = table.Column<Guid>(name: "post_id", type: "uuid", nullable: false),
                    postfriendlyid = table.Column<string>(name: "post_friendly_id", type: "character varying(12)", maxLength: 12, nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    labelid = table.Column<Guid>(name: "label_id", type: "uuid", nullable: true),
                    ismemberonly = table.Column<bool>(name: "is_member_only", type: "boolean", nullable: false),
                    isspoiler = table.Column<bool>(name: "is_spoiler", type: "boolean", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    titlenormalized = table.Column<string>(name: "title_normalized", type: "character varying(250)", maxLength: 250, nullable: false),
                    text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "title", "text" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "title", "text" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post", x => x.postid);
                    table.UniqueConstraint("ak_post_post_friendly_id", x => x.postfriendlyid);
                    table.ForeignKey(
                        name: "fk_post_asp_net_users_creator_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_post_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_post_label_label_id",
                        column: x => x.labelid,
                        principalSchema: "libellus_social",
                        principalTable: "label",
                        principalColumn: "label_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "book",
                schema: "libellus_social",
                columns: table => new
                {
                    bookid = table.Column<Guid>(name: "book_id", type: "uuid", nullable: false),
                    bookfriendlyid = table.Column<string>(name: "book_friendly_id", type: "character varying(12)", maxLength: 12, nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    coverimageid = table.Column<Guid>(name: "cover_image_id", type: "uuid", nullable: false),
                    literatureformid = table.Column<Guid>(name: "literature_form_id", type: "uuid", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    titlenormalized = table.Column<string>(name: "title_normalized", type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book", x => x.bookid);
                    table.UniqueConstraint("ak_book_book_friendly_id", x => x.bookfriendlyid);
                    table.ForeignKey(
                        name: "fk_book_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_book_groups_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_literature_forms_literature_form_id",
                        column: x => x.literatureformid,
                        principalSchema: "libellus_social",
                        principalTable: "literature_form",
                        principalColumn: "literature_form_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                schema: "libellus_social",
                columns: table => new
                {
                    commentid = table.Column<Guid>(name: "comment_id", type: "uuid", nullable: false),
                    commentfriendlyid = table.Column<string>(name: "comment_friendly_id", type: "character varying(15)", maxLength: 15, nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: false),
                    postid = table.Column<Guid>(name: "post_id", type: "uuid", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comment", x => x.commentid);
                    table.UniqueConstraint("ak_comment_comment_friendly_id", x => x.commentfriendlyid);
                    table.ForeignKey(
                        name: "fk_comment_asp_net_users_creator_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comment_groups_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comment_posts_post_id",
                        column: x => x.postid,
                        principalSchema: "libellus_social",
                        principalTable: "post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "book_author_connector",
                schema: "libellus_social",
                columns: table => new
                {
                    bookid = table.Column<Guid>(name: "book_id", type: "uuid", nullable: false),
                    authorid = table.Column<Guid>(name: "author_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_author_connector", x => new { x.bookid, x.authorid });
                    table.ForeignKey(
                        name: "fk_book_author_connector_author_author_id",
                        column: x => x.authorid,
                        principalSchema: "libellus_social",
                        principalTable: "author",
                        principalColumn: "author_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_author_connector_books_book_id",
                        column: x => x.bookid,
                        principalSchema: "libellus_social",
                        principalTable: "book",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "book_edition",
                schema: "libellus_social",
                columns: table => new
                {
                    bookeditionid = table.Column<Guid>(name: "book_edition_id", type: "uuid", nullable: false),
                    bookeditionfriendlyid = table.Column<string>(name: "book_edition_friendly_id", type: "character varying(12)", maxLength: 12, nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    bookid = table.Column<Guid>(name: "book_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: true),
                    coverimageid = table.Column<Guid>(name: "cover_image_id", type: "uuid", nullable: false),
                    formatid = table.Column<Guid>(name: "format_id", type: "uuid", nullable: true),
                    languageid = table.Column<Guid>(name: "language_id", type: "uuid", nullable: true),
                    publisherid = table.Column<Guid>(name: "publisher_id", type: "uuid", nullable: true),
                    publishedonyear = table.Column<int>(name: "published_on_year", type: "integer", nullable: true),
                    publishedonmonth = table.Column<int>(name: "published_on_month", type: "integer", nullable: true),
                    publishedonday = table.Column<int>(name: "published_on_day", type: "integer", nullable: true),
                    publishedon = table.Column<Instant>(name: "published_on", type: "timestamp with time zone", nullable: true),
                    pagecount = table.Column<int>(name: "page_count", type: "integer", nullable: true),
                    wordcount = table.Column<int>(name: "word_count", type: "integer", nullable: true),
                    isbn = table.Column<string>(type: "text", nullable: true),
                    istranslation = table.Column<bool>(name: "is_translation", type: "boolean", nullable: false),
                    isfromlibrary = table.Column<bool>(name: "is_from_library", type: "boolean", nullable: false),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    titlenormalized = table.Column<string>(name: "title_normalized", type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    searchvectorsimple = table.Column<NpgsqlTsVector>(name: "search_vector_simple", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "simple")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" }),
                    searchvectorcustom = table.Column<NpgsqlTsVector>(name: "search_vector_custom", type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "title", "description" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_edition", x => x.bookeditionid);
                    table.UniqueConstraint("ak_book_edition_book_edition_friendly_id", x => x.bookeditionfriendlyid);
                    table.ForeignKey(
                        name: "fk_book_edition_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_book_edition_book_book_id",
                        column: x => x.bookid,
                        principalSchema: "libellus_social",
                        principalTable: "book",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_edition_formats_format_id",
                        column: x => x.formatid,
                        principalSchema: "libellus_social",
                        principalTable: "format",
                        principalColumn: "format_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_book_edition_groups_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_edition_languages_language_id",
                        column: x => x.languageid,
                        principalSchema: "libellus_social",
                        principalTable: "language",
                        principalColumn: "language_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_book_edition_publishers_publisher_id",
                        column: x => x.publisherid,
                        principalSchema: "libellus_social",
                        principalTable: "publisher",
                        principalColumn: "publisher_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "book_genre_connector",
                schema: "libellus_social",
                columns: table => new
                {
                    bookid = table.Column<Guid>(name: "book_id", type: "uuid", nullable: false),
                    genreid = table.Column<Guid>(name: "genre_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_genre_connector", x => new { x.bookid, x.genreid });
                    table.ForeignKey(
                        name: "fk_book_genre_connector_book_book_id",
                        column: x => x.bookid,
                        principalSchema: "libellus_social",
                        principalTable: "book",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_genre_connector_genres_genre_id",
                        column: x => x.genreid,
                        principalSchema: "libellus_social",
                        principalTable: "genre",
                        principalColumn: "genre_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "book_series_connector",
                schema: "libellus_social",
                columns: table => new
                {
                    bookid = table.Column<Guid>(name: "book_id", type: "uuid", nullable: false),
                    seriesid = table.Column<Guid>(name: "series_id", type: "uuid", nullable: false),
                    numberinseries = table.Column<decimal>(name: "number_in_series", type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_series_connector", x => new { x.bookid, x.seriesid });
                    table.ForeignKey(
                        name: "fk_book_series_connector_book_book_id",
                        column: x => x.bookid,
                        principalSchema: "libellus_social",
                        principalTable: "book",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_series_connector_series_series_id",
                        column: x => x.seriesid,
                        principalSchema: "libellus_social",
                        principalTable: "series",
                        principalColumn: "series_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "book_tag_connector",
                schema: "libellus_social",
                columns: table => new
                {
                    bookid = table.Column<Guid>(name: "book_id", type: "uuid", nullable: false),
                    tagid = table.Column<Guid>(name: "tag_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_tag_connector", x => new { x.bookid, x.tagid });
                    table.ForeignKey(
                        name: "fk_book_tag_connector_book_book_id",
                        column: x => x.bookid,
                        principalSchema: "libellus_social",
                        principalTable: "book",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_tag_connector_tags_tag_id",
                        column: x => x.tagid,
                        principalSchema: "libellus_social",
                        principalTable: "tag",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "book_warning_tag_connector",
                schema: "libellus_social",
                columns: table => new
                {
                    bookid = table.Column<Guid>(name: "book_id", type: "uuid", nullable: false),
                    warningtagid = table.Column<Guid>(name: "warning_tag_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_warning_tag_connector", x => new { x.bookid, x.warningtagid });
                    table.ForeignKey(
                        name: "fk_book_warning_tag_connector_book_book_id",
                        column: x => x.bookid,
                        principalSchema: "libellus_social",
                        principalTable: "book",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_warning_tag_connector_warning_tags_warning_tag_id",
                        column: x => x.warningtagid,
                        principalSchema: "libellus_social",
                        principalTable: "warning_tag",
                        principalColumn: "warning_tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shelf_book_connector",
                schema: "libellus_social",
                columns: table => new
                {
                    shelfid = table.Column<Guid>(name: "shelf_id", type: "uuid", nullable: false),
                    bookid = table.Column<Guid>(name: "book_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shelf_book_connector", x => new { x.shelfid, x.bookid });
                    table.ForeignKey(
                        name: "fk_shelf_book_connector_book_book_id",
                        column: x => x.bookid,
                        principalSchema: "libellus_social",
                        principalTable: "book",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shelf_book_connector_shelves_shelf_id",
                        column: x => x.shelfid,
                        principalSchema: "libellus_social",
                        principalTable: "shelf",
                        principalColumn: "shelf_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reading",
                schema: "libellus_social",
                columns: table => new
                {
                    readingid = table.Column<Guid>(name: "reading_id", type: "uuid", nullable: false),
                    readingfriendlyid = table.Column<string>(name: "reading_friendly_id", type: "character varying(12)", maxLength: 12, nullable: false),
                    groupid = table.Column<Guid>(name: "group_id", type: "uuid", nullable: false),
                    creatorid = table.Column<Guid>(name: "creator_id", type: "uuid", nullable: false),
                    bookeditionid = table.Column<Guid>(name: "book_edition_id", type: "uuid", nullable: false),
                    noteid = table.Column<Guid>(name: "note_id", type: "uuid", nullable: true),
                    isdnf = table.Column<bool>(name: "is_dnf", type: "boolean", nullable: false),
                    isreread = table.Column<bool>(name: "is_reread", type: "boolean", nullable: false),
                    score = table.Column<double>(type: "double precision", precision: 7, scale: 4, nullable: true),
                    startedonutc = table.Column<ZonedDateTime>(name: "started_on_utc", type: "timestamp with time zone", nullable: true),
                    finishedonutc = table.Column<ZonedDateTime>(name: "finished_on_utc", type: "timestamp with time zone", nullable: true),
                    createdonutc = table.Column<ZonedDateTime>(name: "created_on_utc", type: "timestamp with time zone", nullable: false),
                    modifiedonutc = table.Column<ZonedDateTime>(name: "modified_on_utc", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reading", x => x.readingid);
                    table.UniqueConstraint("ak_reading_reading_friendly_id", x => x.readingfriendlyid);
                    table.ForeignKey(
                        name: "fk_reading_asp_net_users_user_id",
                        column: x => x.creatorid,
                        principalSchema: "libellus_security",
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reading_book_edition_book_edition_id",
                        column: x => x.bookeditionid,
                        principalSchema: "libellus_social",
                        principalTable: "book_edition",
                        principalColumn: "book_edition_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reading_group_group_id",
                        column: x => x.groupid,
                        principalSchema: "libellus_social",
                        principalTable: "group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reading_note_note_id",
                        column: x => x.noteid,
                        principalSchema: "libellus_social",
                        principalTable: "note",
                        principalColumn: "note_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                schema: "libellus_security",
                table: "asp_net_role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "libellus_security",
                table: "asp_net_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                schema: "libellus_security",
                table: "asp_net_user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                schema: "libellus_security",
                table: "asp_net_user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                schema: "libellus_security",
                table: "asp_net_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "libellus_security",
                table: "asp_net_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "libellus_security",
                table: "asp_net_users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_author_creator_id",
                schema: "libellus_social",
                table: "author",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_author_group_id",
                schema: "libellus_social",
                table: "author",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_author_search_vector_custom",
                schema: "libellus_social",
                table: "author",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_author_search_vector_simple",
                schema: "libellus_social",
                table: "author",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_book_creator_id",
                schema: "libellus_social",
                table: "book",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_group_id_book_id_literature_form_id",
                schema: "libellus_social",
                table: "book",
                columns: new[] { "group_id", "book_id", "literature_form_id" });

            migrationBuilder.CreateIndex(
                name: "ix_book_group_id_book_id_title_normalized",
                schema: "libellus_social",
                table: "book",
                columns: new[] { "group_id", "book_id", "title_normalized" });

            migrationBuilder.CreateIndex(
                name: "ix_book_literature_form_id",
                schema: "libellus_social",
                table: "book",
                column: "literature_form_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_search_vector_custom",
                schema: "libellus_social",
                table: "book",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_book_search_vector_simple",
                schema: "libellus_social",
                table: "book",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_book_author_connector_author_id",
                schema: "libellus_social",
                table: "book_author_connector",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_book_edition_id_published_on",
                schema: "libellus_social",
                table: "book_edition",
                columns: new[] { "book_edition_id", "published_on" });

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_book_id",
                schema: "libellus_social",
                table: "book_edition",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_creator_id",
                schema: "libellus_social",
                table: "book_edition",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_format_id",
                schema: "libellus_social",
                table: "book_edition",
                column: "format_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_group_id_book_edition_id_format_id",
                schema: "libellus_social",
                table: "book_edition",
                columns: new[] { "group_id", "book_edition_id", "format_id" });

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_group_id_book_edition_id_isbn",
                schema: "libellus_social",
                table: "book_edition",
                columns: new[] { "group_id", "book_edition_id", "isbn" });

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_group_id_book_edition_id_language_id",
                schema: "libellus_social",
                table: "book_edition",
                columns: new[] { "group_id", "book_edition_id", "language_id" });

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_group_id_book_edition_id_publisher_id",
                schema: "libellus_social",
                table: "book_edition",
                columns: new[] { "group_id", "book_edition_id", "publisher_id" });

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_group_id_book_edition_id_title_normalized",
                schema: "libellus_social",
                table: "book_edition",
                columns: new[] { "group_id", "book_edition_id", "title_normalized" });

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_language_id",
                schema: "libellus_social",
                table: "book_edition",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_publisher_id",
                schema: "libellus_social",
                table: "book_edition",
                column: "publisher_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_search_vector_custom",
                schema: "libellus_social",
                table: "book_edition",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_book_edition_search_vector_simple",
                schema: "libellus_social",
                table: "book_edition",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_book_genre_connector_genre_id",
                schema: "libellus_social",
                table: "book_genre_connector",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_series_connector_book_id",
                schema: "libellus_social",
                table: "book_series_connector",
                column: "book_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_book_series_connector_series_id",
                schema: "libellus_social",
                table: "book_series_connector",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_tag_connector_tag_id",
                schema: "libellus_social",
                table: "book_tag_connector",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_book_warning_tag_connector_warning_tag_id",
                schema: "libellus_social",
                table: "book_warning_tag_connector",
                column: "warning_tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_creator_id",
                schema: "libellus_social",
                table: "comment",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_group_id",
                schema: "libellus_social",
                table: "comment",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_post_id",
                schema: "libellus_social",
                table: "comment",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_cover_image_meta_data_object_name",
                schema: "libellus_media",
                table: "cover_image_meta_data",
                column: "object_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cover_image_meta_data_public_id_cover_image_meta_data_id",
                schema: "libellus_media",
                table: "cover_image_meta_data",
                columns: new[] { "public_id", "cover_image_meta_data_id" });

            migrationBuilder.CreateIndex(
                name: "ix_format_creator_id",
                schema: "libellus_social",
                table: "format",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_format_group_id_name_normalized",
                schema: "libellus_social",
                table: "format",
                columns: new[] { "group_id", "name_normalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_format_search_vector_custom",
                schema: "libellus_social",
                table: "format",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_format_search_vector_simple",
                schema: "libellus_social",
                table: "format",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_genre_creator_id",
                schema: "libellus_social",
                table: "genre",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_genre_group_id_name_normalized",
                schema: "libellus_social",
                table: "genre",
                columns: new[] { "group_id", "name_normalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_genre_search_vector_custom",
                schema: "libellus_social",
                table: "genre",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_genre_search_vector_simple",
                schema: "libellus_social",
                table: "genre",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_group_group_id_name_normalized",
                schema: "libellus_social",
                table: "group",
                columns: new[] { "group_id", "name_normalized" });

            migrationBuilder.CreateIndex(
                name: "ix_group_search_vector_custom",
                schema: "libellus_social",
                table: "group",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_group_search_vector_simple",
                schema: "libellus_social",
                table: "group",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_group_role_group_role_id_name",
                schema: "libellus_security",
                table: "group_role",
                columns: new[] { "group_role_id", "name" });

            migrationBuilder.CreateIndex(
                name: "ix_group_role_name",
                schema: "libellus_security",
                table: "group_role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_group_user_membership_group_role_id",
                schema: "libellus_social",
                table: "group_user_membership",
                column: "group_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_user_membership_user_id",
                schema: "libellus_social",
                table: "group_user_membership",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitation_group_id_invitation_id",
                schema: "libellus_social",
                table: "invitation",
                columns: new[] { "group_id", "invitation_id" });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_invitation_id_invitation_status",
                schema: "libellus_social",
                table: "invitation",
                columns: new[] { "invitation_id", "invitation_status" },
                filter: "invitation_status = 1")
                .Annotation("Npgsql:IndexInclude", new[] { "created_on_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_invitee_id_invitation_id",
                schema: "libellus_social",
                table: "invitation",
                columns: new[] { "invitee_id", "invitation_id" });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_inviter_id",
                schema: "libellus_social",
                table: "invitation",
                column: "inviter_id");

            migrationBuilder.CreateIndex(
                name: "ix_label_group_id_name_normalized",
                schema: "libellus_social",
                table: "label",
                columns: new[] { "group_id", "name_normalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_label_search_vector_custom",
                schema: "libellus_social",
                table: "label",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_label_search_vector_simple",
                schema: "libellus_social",
                table: "label",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_language_creator_id",
                schema: "libellus_social",
                table: "language",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_language_group_id_name_normalized",
                schema: "libellus_social",
                table: "language",
                columns: new[] { "group_id", "name_normalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_language_search_vector_custom",
                schema: "libellus_social",
                table: "language",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_language_search_vector_simple",
                schema: "libellus_social",
                table: "language",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_literature_form_creator_id",
                schema: "libellus_social",
                table: "literature_form",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_literature_form_group_id_name_normalized",
                schema: "libellus_social",
                table: "literature_form",
                columns: new[] { "group_id", "name_normalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_literature_form_search_vector_custom",
                schema: "libellus_social",
                table: "literature_form",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_literature_form_search_vector_simple",
                schema: "libellus_social",
                table: "literature_form",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_note_creator_id",
                schema: "libellus_social",
                table: "note",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_note_group_id",
                schema: "libellus_social",
                table: "note",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_creator_id",
                schema: "libellus_social",
                table: "post",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_group_id_post_id_label_id",
                schema: "libellus_social",
                table: "post",
                columns: new[] { "group_id", "post_id", "label_id" });

            migrationBuilder.CreateIndex(
                name: "ix_post_group_id_title_normalized",
                schema: "libellus_social",
                table: "post",
                columns: new[] { "group_id", "title_normalized" });

            migrationBuilder.CreateIndex(
                name: "ix_post_label_id",
                schema: "libellus_social",
                table: "post",
                column: "label_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_search_vector_custom",
                schema: "libellus_social",
                table: "post",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_post_search_vector_simple",
                schema: "libellus_social",
                table: "post",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_post_title_normalized",
                schema: "libellus_social",
                table: "post",
                column: "title_normalized")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "simple");

            migrationBuilder.CreateIndex(
                name: "ix_profile_picture_meta_data_object_name",
                schema: "libellus_media",
                table: "profile_picture_meta_data",
                column: "object_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_profile_picture_meta_data_public_id_profile_picture_meta_da",
                schema: "libellus_media",
                table: "profile_picture_meta_data",
                columns: new[] { "public_id", "profile_picture_meta_data_id" });

            migrationBuilder.CreateIndex(
                name: "ix_publisher_creator_id",
                schema: "libellus_social",
                table: "publisher",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_publisher_group_id_name_normalized",
                schema: "libellus_social",
                table: "publisher",
                columns: new[] { "group_id", "name_normalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_publisher_search_vector_custom",
                schema: "libellus_social",
                table: "publisher",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_publisher_search_vector_simple",
                schema: "libellus_social",
                table: "publisher",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_reading_book_edition_id",
                schema: "libellus_social",
                table: "reading",
                column: "book_edition_id");

            migrationBuilder.CreateIndex(
                name: "ix_reading_creator_id",
                schema: "libellus_social",
                table: "reading",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_reading_group_id",
                schema: "libellus_social",
                table: "reading",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_reading_note_id",
                schema: "libellus_social",
                table: "reading",
                column: "note_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_series_creator_id",
                schema: "libellus_social",
                table: "series",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_series_group_id_title_normalized",
                schema: "libellus_social",
                table: "series",
                columns: new[] { "group_id", "title_normalized" });

            migrationBuilder.CreateIndex(
                name: "ix_series_search_vector_custom",
                schema: "libellus_social",
                table: "series",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_series_search_vector_simple",
                schema: "libellus_social",
                table: "series",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_series_title_normalized",
                schema: "libellus_social",
                table: "series",
                column: "title_normalized")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "simple");

            migrationBuilder.CreateIndex(
                name: "ix_shelf_creator_id",
                schema: "libellus_social",
                table: "shelf",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_shelf_group_id_name_normalized",
                schema: "libellus_social",
                table: "shelf",
                columns: new[] { "group_id", "name_normalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_shelf_search_vector_custom",
                schema: "libellus_social",
                table: "shelf",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_shelf_search_vector_simple",
                schema: "libellus_social",
                table: "shelf",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_shelf_book_connector_book_id",
                schema: "libellus_social",
                table: "shelf_book_connector",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "ix_tag_creator_id",
                schema: "libellus_social",
                table: "tag",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_tag_group_id_name_normalized",
                schema: "libellus_social",
                table: "tag",
                columns: new[] { "group_id", "name_normalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tag_search_vector_custom",
                schema: "libellus_social",
                table: "tag",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_tag_search_vector_simple",
                schema: "libellus_social",
                table: "tag",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_warning_tag_creator_id",
                schema: "libellus_social",
                table: "warning_tag",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_warning_tag_group_id_name_normalized",
                schema: "libellus_social",
                table: "warning_tag",
                columns: new[] { "group_id", "name_normalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_warning_tag_search_vector_custom",
                schema: "libellus_social",
                table: "warning_tag",
                column: "search_vector_custom")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "ix_warning_tag_search_vector_simple",
                schema: "libellus_social",
                table: "warning_tag",
                column: "search_vector_simple")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asp_net_role_claims",
                schema: "libellus_security");

            migrationBuilder.DropTable(
                name: "asp_net_user_claims",
                schema: "libellus_security");

            migrationBuilder.DropTable(
                name: "asp_net_user_logins",
                schema: "libellus_security");

            migrationBuilder.DropTable(
                name: "asp_net_user_roles",
                schema: "libellus_security");

            migrationBuilder.DropTable(
                name: "asp_net_user_tokens",
                schema: "libellus_security");

            migrationBuilder.DropTable(
                name: "book_author_connector",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "book_genre_connector",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "book_series_connector",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "book_tag_connector",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "book_warning_tag_connector",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "comment",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "cover_image_meta_data",
                schema: "libellus_media");

            migrationBuilder.DropTable(
                name: "group_user_membership",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "invitation",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "profile_picture_meta_data",
                schema: "libellus_media");

            migrationBuilder.DropTable(
                name: "reading",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "shelf_book_connector",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "asp_net_roles",
                schema: "libellus_security");

            migrationBuilder.DropTable(
                name: "author",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "genre",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "series",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "tag",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "warning_tag",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "post",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "group_role",
                schema: "libellus_security");

            migrationBuilder.DropTable(
                name: "book_edition",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "note",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "shelf",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "label",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "book",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "format",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "language",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "publisher",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "literature_form",
                schema: "libellus_social");

            migrationBuilder.DropTable(
                name: "asp_net_users",
                schema: "libellus_security");

            migrationBuilder.DropTable(
                name: "group",
                schema: "libellus_social");
        }
    }
}
