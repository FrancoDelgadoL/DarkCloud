using System.Collections.Generic;

namespace DarkCloud.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public bool EsOferta { get; set; }
        public decimal? PrecioReal { get; set; }
        public decimal? Descuento { get; set; }
        // Nuevos campos para clasificación
        public string? Genero { get; set; }
        public string? Plataforma { get; set; }
        public string? TipoProducto { get; set; }
        public string? ModoJuego { get; set; }
        public string? ClasificacionEdad { get; set; }
        public string? CategoriasEspeciales { get; set; } // Puede ser múltiple, separado por comas

        public DateTime? OfertaInicio { get; set; }
        public DateTime? OfertaFin { get; set; }
        public int? DuracionOfertaHoras { get; set; }

        public virtual ICollection<ProductoImagen> Imagenes { get; set; } = new List<ProductoImagen>();

        public byte[]? ImagenPrincipal { get; set; }
        public string? ImagenPrincipalMimeType { get; set; }
    }
}
