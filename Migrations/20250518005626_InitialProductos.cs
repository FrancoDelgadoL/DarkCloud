using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DarkCloud.Migrations
{
    /// <inheritdoc />
    public partial class InitialProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Descuento = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
