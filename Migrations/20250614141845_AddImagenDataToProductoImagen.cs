using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkCloud.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenDataToProductoImagen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImagenData",
                table: "ProductoImagenes",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenMimeType",
                table: "ProductoImagenes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenData",
                table: "ProductoImagenes");

            migrationBuilder.DropColumn(
                name: "ImagenMimeType",
                table: "ProductoImagenes");
        }
    }
}
