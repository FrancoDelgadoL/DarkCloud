using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkCloud.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        [Required]
        public string UsuarioId { get; set; } = null!;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Pagado, Enviado, etc.
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    }
}
