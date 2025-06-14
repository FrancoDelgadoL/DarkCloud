using Microsoft.AspNetCore.Mvc;
using DarkCloud.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace DarkCloud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiPublicController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ApiPublicController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos los productos disponibles.
        /// </summary>
        [HttpGet("productos")]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _context.Productos.ToListAsync();
            return Ok(productos);
        }

        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        [HttpGet("producto/{id}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        /// <summary>
        /// Lista todos los usuarios (solo para demostración, no exponer en producción).
        /// </summary>
        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Users.Select(u => new { u.Id, u.Nombre, u.Apellido, u.Email, u.Rol }).ToListAsync();
            return Ok(usuarios);
        }

        /// <summary>
        /// Lista todos los pedidos.
        /// </summary>
        [HttpGet("pedidos")]
        public async Task<IActionResult> GetPedidos()
        {
            var pedidos = await _context.Pedidos.Include(p => p.Detalles).ToListAsync();
            return Ok(pedidos);
        }

        /// <summary>
        /// Endpoint de prueba para Swagger.
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { mensaje = "API funcionando correctamente" });
        }
    }
}
