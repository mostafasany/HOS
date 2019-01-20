using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nop.Plugin.Api.Common.Migrations.IdentityServer.ConfigurationDb
{
    public partial class DotNetCore2IdentityServerConfigurationDbMigration : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ClientProperties");

            migrationBuilder.DropColumn(
                "BackChannelLogoutSessionRequired",
                "Clients");

            migrationBuilder.DropColumn(
                "BackChannelLogoutUri",
                "Clients");

            migrationBuilder.DropColumn(
                "ClientClaimsPrefix",
                "Clients");

            migrationBuilder.DropColumn(
                "ConsentLifetime",
                "Clients");

            migrationBuilder.DropColumn(
                "Description",
                "Clients");

            migrationBuilder.DropColumn(
                "FrontChannelLogoutSessionRequired",
                "Clients");

            migrationBuilder.DropColumn(
                "FrontChannelLogoutUri",
                "Clients");

            migrationBuilder.DropColumn(
                "PairWiseSubjectSalt",
                "Clients");

            migrationBuilder.AlterColumn<string>(
                "LogoUri",
                "Clients",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                "LogoutSessionRequired",
                "Clients",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                "LogoutUri",
                "Clients",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                "PrefixClientClaims",
                "Clients",
                nullable: false,
                defaultValue: false);
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "LogoutSessionRequired",
                "Clients");

            migrationBuilder.DropColumn(
                "LogoutUri",
                "Clients");

            migrationBuilder.DropColumn(
                "PrefixClientClaims",
                "Clients");

            migrationBuilder.AlterColumn<string>(
                "LogoUri",
                "Clients",
                "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                "BackChannelLogoutSessionRequired",
                "Clients",
                "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                "BackChannelLogoutUri",
                "Clients",
                "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "ClientClaimsPrefix",
                "Clients",
                "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                "ConsentLifetime",
                "Clients",
                "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "Description",
                "Clients",
                "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                "FrontChannelLogoutSessionRequired",
                "Clients",
                "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                "FrontChannelLogoutUri",
                "Clients",
                "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "PairWiseSubjectSalt",
                "Clients",
                "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                "ClientProperties",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>("int", nullable: false),
                    Key = table.Column<string>("nvarchar(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>("nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProperties", x => x.Id);
                    table.ForeignKey(
                        "FK_ClientProperties_Clients_ClientId",
                        x => x.ClientId,
                        "Clients",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_ClientProperties_ClientId",
                "ClientProperties",
                "ClientId");
        }
    }
}