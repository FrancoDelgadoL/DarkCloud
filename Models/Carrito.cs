using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarkCloud.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        [Required]
        public string UsuarioId { get; set; } = null!;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool Activo { get; set; } = true;
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual ICollection<CarritoDetalle> Detalles { get; set; } = new List<CarritoDetalle>();
    }
}
