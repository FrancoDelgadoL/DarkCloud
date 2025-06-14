using Microsoft.AspNetCore.Mvc;
using DarkCloud.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Security.Claims;

namespace DarkCloud.Controllers
{
    public class PagoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> IniciarPago()
        {
            // Obtener usuario autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            // Obtener el carrito del usuario
            var carrito = _context.Carritos
                .Include(c => c.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefault(c => c.UsuarioId == userId);

            if (carrito == null || carrito.Detalles == null || !carrito.Detalles.Any())
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "Carrito");
            }

            // Construir el JSON de preferencia para Mercado Pago
            var items = carrito.Detalles.Select(detalle => new {
                title = detalle.Producto?.Nombre ?? "Producto",
                quantity = detalle.Cantidad,
                currency_id = "PEN",
                unit_price = (decimal)(detalle.Producto?.Precio ?? 0)
            }).ToList();

            // URLs absolutas manuales, sin Url.Action
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            // PRUEBA: URL de éxito hardcodeada para descartar problemas de generación local
            var successUrl = "https://www.google.com";
            var failureUrl = baseUrl + "/Pago/Error";
            var pendingUrl = baseUrl + "/Pago/Pendiente";

            var preferencia = new {
                items = items,
                back_urls = new {
                    success = successUrl,
                    failure = failureUrl,
                    pending = pendingUrl
                },
                auto_return = "approved"
            };
            TempData["Error"] = $"JSON enviado: {JsonSerializer.Serialize(preferencia)}";

            var json = JsonSerializer.Serialize(preferencia);
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TEST-4115979895821777-061412-8f2e907b7ca78808f8716ccfc00c5bfb-2494164889");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await http.PostAsync("https://api.mercadopago.com/checkout/preferences", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"No se pudo iniciar el pago. Detalle: {errorMsg}";
                return RedirectToAction("Index", "Carrito");
            }
            using var respStream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(respStream);
            var initPoint = doc.RootElement.GetProperty("init_point").GetString();
            return Redirect(initPoint);
        }

        [HttpGet]
        public IActionResult Exito() => View();
        [HttpGet]
        public IActionResult Pendiente() => View();
        [HttpGet]
        public IActionResult Error() => View();
    }
}
