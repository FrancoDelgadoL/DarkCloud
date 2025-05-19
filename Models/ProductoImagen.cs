using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkCloud.Models
{
    public class ProductoImagen
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductoId { get; set; }
        [Required]
        [MaxLength(300)]
        public string Url { get; set; } = string.Empty;
        public int Orden { get; set; } = 0;
        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }
    }
}
