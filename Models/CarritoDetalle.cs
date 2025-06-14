using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkCloud.Models
{
    public class CarritoDetalle
    {
        public int Id { get; set; }
        [Required]
        public int CarritoId { get; set; }
        [Required]
        public int ProductoId { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [ForeignKey("CarritoId")]
        public virtual Carrito Carrito { get; set; } = null!;
        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; } = null!;
    }
}
