using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mira.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StardewCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Villager = table.Column<string>(type: "text", nullable: false),
                    Birthday = table.Column<string>(type: "text", nullable: false),
                    Loves = table.Column<string>(type: "text", nullable: false),
                    Likes = table.Column<string>(type: "text", nullable: false),
                    Neutral = table.Column<string>(type: "text", nullable: false),
                    Dislikes = table.Column<string>(type: "text", nullable: false),
                    Hates = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StardewCharacters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StardewCharacters");
        }
    }
}
