using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkCloud.Models
{
    public class PedidoDetalle
    {
        public int Id { get; set; }
        [Required]
        public int PedidoId { get; set; }
        [Required]
        public int ProductoId { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public decimal PrecioUnitario { get; set; }
        [ForeignKey("PedidoId")]
        public virtual Pedido Pedido { get; set; } = null!;
        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; } = null!;
    }
}
