using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DarkCloud.Models;

namespace DarkCloud.Models
{
    public class ApplicationDbContext : IdentityDbContext<Usuario, Microsoft.AspNetCore.Identity.IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<HomeHeroConfig> HomeHeroConfigs { get; set; }
        public DbSet<ProductoImagen> ProductoImagenes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoDetalle> CarritoDetalles { get; set; }
        public DbSet<HomeHeroImagen> HomeHeroImagenes { get; set; }
        // DbSet<Usuario> ya no es necesario, Identity lo gestiona
    }
}