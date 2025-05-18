using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkCloud.Migrations
{
    /// <inheritdoc />
    public partial class ClasificacionProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoriasEspeciales",
                table: "Productos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClasificacionEdad",
                table: "Productos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "Productos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModoJuego",
                table: "Productos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plataforma",
                table: "Productos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoProducto",
                table: "Productos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriasEspeciales",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ClasificacionEdad",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Genero",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ModoJuego",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Plataforma",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "TipoProducto",
                table: "Productos");
        }
    }
}
