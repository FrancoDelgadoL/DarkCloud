using System;
using System.Collections.Generic;

namespace DarkCloud.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal Total { get; set; }

        public required Usuario Usuario { get; set; }
        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}