using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkCloud.Models
{
    public class HomeHeroImagen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public byte[] Imagen { get; set; }

        [Required]
        public string MimeType { get; set; }

        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public int Orden { get; set; }

        // Relación con HomeHeroConfig
        public int HomeHeroConfigId { get; set; }
        [ForeignKey("HomeHeroConfigId")]
        public HomeHeroConfig HomeHeroConfig { get; set; }
    }
}
