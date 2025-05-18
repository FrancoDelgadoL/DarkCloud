using System;
using System.Collections.Generic;

namespace DarkCloud.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime FechaRegistro { get; set; }

        public ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}