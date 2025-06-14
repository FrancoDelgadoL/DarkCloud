using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkCloud.Models
{
    public class HomeHeroConfig
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Titulo { get; set; }
        [Column(TypeName = "text")]
        public string? FrasesJson { get; set; } // JSON array de frases
        [Column(TypeName = "text")]
        public string? ImagenesCarruselJson { get; set; } // JSON array de imágenes
        [Column(TypeName = "text")]
        public string? DuracionesJson { get; set; } // JSON array de duraciones (ms) para cada frase
        public int DuracionCarruselMs { get; set; } = 5000; // fallback global
        [MaxLength(100)]
        public string? TextoBoton { get; set; }
        [MaxLength(200)]
        public string? EnlaceBoton { get; set; }
        [MaxLength(300)]
        public string? MensajeBanner { get; set; }

        public virtual ICollection<HomeHeroImagen> ImagenesCarrusel { get; set; } = new List<HomeHeroImagen>();
    }
}
