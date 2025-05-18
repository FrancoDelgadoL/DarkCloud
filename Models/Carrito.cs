using System;

namespace DarkCloud.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }

        public Usuario Usuario { get; set; }
        public Producto Producto { get; set; }
    }
}