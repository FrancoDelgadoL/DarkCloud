using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DarkCloud.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    DuracionesJson = table.Column<string>(type: "text", nullable: true),
                    DuracionCarruselMs = table.Column<int>(type: "integer", nullable: false),
                    TextoBoton = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EnlaceBoton = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MensajeBanner = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeHeroConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Precio = table.Column<decimal>(type: "numeric", nullable: false),
                    ImagenUrl = table.Column<string>(type: "text", nullable: true),
                    EsOferta = table.Column<bool>(type: "boolean", nullable: false),
                    PrecioReal = table.Column<decimal>(type: "numeric", nullable: true),
                    Descuento = table.Column<decimal>(type: "numeric", nullable: true),
                    Genero = table.Column<string>(type: "text", nullable: true),
                    Plataforma = table.Column<string>(type: "text", nullable: true),
                    TipoProducto = table.Column<string>(type: "text", nullable: true),
                    ModoJuego = table.Column<string>(type: "text", nullable: true),
                    ClasificacionEdad = table.Column<string>(type: "text", nullable: true),
                    CategoriasEspeciales = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductoImagenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoImagenes_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoImagenes_ProductoId",
                table: "ProductoImagenes",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeHeroConfigs");

            migrationBuilder.DropTable(
                name: "ProductoImagenes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
