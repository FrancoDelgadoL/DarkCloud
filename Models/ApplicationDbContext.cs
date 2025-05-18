using Microsoft.EntityFrameworkCore;

namespace DarkCloud.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<HomeHeroConfig> HomeHeroConfigs { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}