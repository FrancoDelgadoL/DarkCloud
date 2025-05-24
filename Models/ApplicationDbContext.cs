using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DarkCloud.Models;

namespace DarkCloud.Models
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<HomeHeroConfig> HomeHeroConfigs { get; set; }
        public DbSet<ProductoImagen> ProductoImagenes { get; set; }
        // DbSet<Usuario> ya no es necesario, Identity lo gestiona
    }
}