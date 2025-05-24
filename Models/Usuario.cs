using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DarkCloud.Models
{
    public class Usuario : IdentityUser
    {
        [Required, MaxLength(100)]
        public required string Nombre { get; set; }
        [Required, MaxLength(100)]
        public required string Apellido { get; set; }
        [Required, MaxLength(20)]
        public required string Rol { get; set; } // "Cliente" o "Administrador"
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
