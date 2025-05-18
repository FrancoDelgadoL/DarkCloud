using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DarkCloud.Migrations
{
    /// <inheritdoc />
    public partial class HomeHeroConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeHeroConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FrasesJson = table.Column<string>(type: "text", nullable: true),
                    ImagenesCarruselJson = table.Column<string>(type: "text", nullable: true),
                    DuracionCarruselMs = table.Column<int>(type: "integer", nullable: false),
                    TextoBoton = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EnlaceBoton = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MensajeBanner = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeHeroConfigs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeHeroConfigs");
        }
    }
}
