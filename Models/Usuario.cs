using System.ComponentModel.DataAnnotations;

namespace DarkCloud.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Nombre { get; set; }
        [Required, MaxLength(100)]
        public string Apellido { get; set; }
        [Required, MaxLength(150)]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required, MaxLength(20)]
        public string Rol { get; set; } // "Cliente" o "Administrador"
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
