using System.Collections.Generic;

namespace DarkCloud.Models
{
    public class HomeViewModel
    {
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public string? Titulo { get; set; }
        public List<string> Frases { get; set; } = new List<string>();
        public List<string> ImagenesCarruselBase64 { get; set; } = new List<string>();
        public int DuracionCarruselMs { get; set; } = 5000;
        public string? TextoBoton { get; set; }
        public string? EnlaceBoton { get; set; }
        public string? MensajeBanner { get; set; }
    }
}
