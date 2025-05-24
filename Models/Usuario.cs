using System.ComponentModel.DataAnnotations;

namespace DarkCloud.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public required string Nombre { get; set; }
        [Required, MaxLength(100)]
        public required string Apellido { get; set; }
        [Required, MaxLength(150)]
        public required string Email { get; set; }
        [Required]
        public required string PasswordHash { get; set; }
        [Required, MaxLength(20)]
        public required string Rol { get; set; } // "Cliente" o "Administrador"
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
