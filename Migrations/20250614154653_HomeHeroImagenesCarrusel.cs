using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DarkCloud.Migrations
{
    /// <inheritdoc />
    public partial class HomeHeroImagenesCarrusel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeHeroImagenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Imagen = table.Column<byte[]>(type: "bytea", nullable: false),
                    MimeType = table.Column<string>(type: "text", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    HomeHeroConfigId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeHeroImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeHeroImagenes_HomeHeroConfigs_HomeHeroConfigId",
                        column: x => x.HomeHeroConfigId,
                        principalTable: "HomeHeroConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeHeroImagenes_HomeHeroConfigId",
                table: "HomeHeroImagenes",
                column: "HomeHeroConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeHeroImagenes");
        }
    }
}
