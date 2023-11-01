using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mira.Migrations
{
    /// <inheritdoc />
    public partial class buh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PremiumStatus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
