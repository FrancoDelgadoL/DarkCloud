using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DarkCloud.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DarkCloud.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CarritoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtiene el carrito activo del usuario
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carrito = await _context.Carritos
                .Include(c => c.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId && c.Activo);
            if (carrito == null)
            {
                carrito = new Carrito { UsuarioId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }
            return View(carrito);
        }

        // Agrega un producto al carrito
        [HttpPost]
        public async Task<IActionResult> Agregar(int productoId, int cantidad = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carrito = await _context.Carritos.Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId && c.Activo);
            if (carrito == null)
            {
                carrito = new Carrito { UsuarioId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }
            var detalle = carrito.Detalles.FirstOrDefault(d => d.ProductoId == productoId);
            if (detalle != null)
            {
                detalle.Cantidad += cantidad;
            }
            else
            {
                carrito.Detalles.Add(new CarritoDetalle { ProductoId = productoId, Cantidad = cantidad });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Quita un producto del carrito
        [HttpPost]
        public async Task<IActionResult> Quitar(int productoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carrito = await _context.Carritos.Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId && c.Activo);
            if (carrito != null)
            {
                var detalle = carrito.Detalles.FirstOrDefault(d => d.ProductoId == productoId);
                if (detalle != null)
                {
                    carrito.Detalles.Remove(detalle);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        // Agrega un producto al carrito (AJAX)
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productoId, int cantidad = 1)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated || !User.IsInRole("Cliente"))
            {
                return Json(new { loginRequired = true });
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { loginRequired = true });
            }
            var carrito = await _context.Carritos.Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId && c.Activo);
            if (carrito == null)
            {
                carrito = new Carrito { UsuarioId = userId };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }
            var detalle = carrito.Detalles.FirstOrDefault(d => d.ProductoId == productoId);
            if (detalle != null)
            {
                detalle.Cantidad += cantidad;
            }
            else
            {
                carrito.Detalles.Add(new CarritoDetalle { ProductoId = productoId, Cantidad = cantidad });
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
