using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DarkCloud.Models;

namespace DarkCloud.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(string? genero, string? plataforma, string? tipo, string? modo, string? clasificacion, string? categoria, string? q)
    {
        var productos = _context.Productos.AsQueryable();
        if (!string.IsNullOrEmpty(genero))
        {
            productos = productos.Where(p => p.Genero == genero);
            ViewData["FiltroGenero"] = genero;
        }
        if (!string.IsNullOrEmpty(plataforma))
        {
            productos = productos.Where(p => p.Plataforma == plataforma);
            ViewData["FiltroPlataforma"] = plataforma;
        }
        if (!string.IsNullOrEmpty(tipo))
        {
            productos = productos.Where(p => p.TipoProducto == tipo);
            ViewData["FiltroTipo"] = tipo;
        }
        if (!string.IsNullOrEmpty(modo))
        {
            productos = productos.Where(p => p.ModoJuego == modo);
            ViewData["FiltroModo"] = modo;
        }
        if (!string.IsNullOrEmpty(clasificacion))
        {
            productos = productos.Where(p => p.ClasificacionEdad == clasificacion);
            ViewData["FiltroClasificacion"] = clasificacion;
        }
        if (!string.IsNullOrEmpty(categoria))
        {
            productos = productos.Where(p => p.CategoriasEspeciales != null && p.CategoriasEspeciales.Contains(categoria));
            ViewData["FiltroCategoria"] = categoria;
        }
        if (!string.IsNullOrEmpty(q))
        {
            var qLower = q.ToLower();
            productos = productos.Where(p =>
                (p.Nombre != null && p.Nombre.ToLower().Contains(qLower)) ||
                (p.Descripcion != null && p.Descripcion.ToLower().Contains(qLower)) ||
                (p.Genero != null && p.Genero.ToLower().Contains(qLower)) ||
                (p.Plataforma != null && p.Plataforma.ToLower().Contains(qLower)) ||
                (p.TipoProducto != null && p.TipoProducto.ToLower().Contains(qLower)) ||
                (p.ModoJuego != null && p.ModoJuego.ToLower().Contains(qLower)) ||
                (p.ClasificacionEdad != null && p.ClasificacionEdad.ToLower().Contains(qLower)) ||
                (p.CategoriasEspeciales != null && p.CategoriasEspeciales.ToLower().Contains(qLower))
            );
            ViewData["Busqueda"] = q;
        }
        var lista = productos.ToList();
        // Lógica de expiración de oferta
        var ahora = DateTime.UtcNow;
        foreach (var p in lista)
        {
            if (p.EsOferta && p.OfertaFin.HasValue && p.OfertaFin.Value < ahora)
            {
                p.EsOferta = false;
                // Solo reasignar si PrecioReal es válido
                if (p.PrecioReal.HasValue && p.PrecioReal.Value > 0)
                {
                    p.Precio = p.PrecioReal.Value;
                }
                // Si PrecioReal no es válido, mantener el precio actual (no poner a cero)
            }
            // Si el precio es cero o negativo, intentar corregir
            if (p.Precio <= 0)
            {
                if (p.PrecioReal.HasValue && p.PrecioReal.Value > 0)
                    p.Precio = p.PrecioReal.Value;
                else if (p.Descuento.HasValue && p.Descuento.Value > 0 && p.PrecioReal.HasValue && p.PrecioReal.Value > 0)
                    p.Precio = p.PrecioReal.Value - (p.PrecioReal.Value * p.Descuento.Value / 100);
                // Si sigue sin ser válido, dejar el precio como está (no poner a cero)
            }
        }
        return View(lista);
    }

    public IActionResult Login()
    {
        ViewData["OcultarFooter"] = true;
        return View();
    }

    public IActionResult Registrar()
    {
        ViewData["OcultarFooter"] = true;
        return View();
    }

    public IActionResult Prod1()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Catalogo(string? genero, string? plataforma, string? tipo, string? modo, string? clasificacion, string? categoria, string? q, bool? novedades)
    {
        var productos = _context.Productos.AsQueryable();
        if (!string.IsNullOrEmpty(genero))
        {
            productos = productos.Where(p => p.Genero == genero);
            ViewData["FiltroGenero"] = genero;
        }
        if (!string.IsNullOrEmpty(plataforma))
        {
            productos = productos.Where(p => p.Plataforma == plataforma);
            ViewData["FiltroPlataforma"] = plataforma;
        }
        if (!string.IsNullOrEmpty(tipo))
        {
            if (tipo == "Oferta")
            {
                productos = productos.Where(p => p.EsOferta);
                ViewData["FiltroTipo"] = "Oferta";
            }
            else
            {
                productos = productos.Where(p => p.TipoProducto == tipo);
                ViewData["FiltroTipo"] = tipo;
            }
        }
        if (!string.IsNullOrEmpty(modo))
        {
            productos = productos.Where(p => p.ModoJuego == modo);
            ViewData["FiltroModo"] = modo;
        }
        if (!string.IsNullOrEmpty(clasificacion))
        {
            productos = productos.Where(p => p.ClasificacionEdad == clasificacion);
            ViewData["FiltroClasificacion"] = clasificacion;
        }
        if (!string.IsNullOrEmpty(categoria))
        {
            productos = productos.Where(p => p.CategoriasEspeciales != null && p.CategoriasEspeciales.Contains(categoria));
            ViewData["FiltroCategoria"] = categoria;
        }
        if (!string.IsNullOrEmpty(q))
        {
            var qLower = q.ToLower();
            productos = productos.Where(p =>
                (p.Nombre != null && p.Nombre.ToLower().Contains(qLower)) ||
                (p.Descripcion != null && p.Descripcion.ToLower().Contains(qLower)) ||
                (p.Genero != null && p.Genero.ToLower().Contains(qLower)) ||
                (p.Plataforma != null && p.Plataforma.ToLower().Contains(qLower)) ||
                (p.TipoProducto != null && p.TipoProducto.ToLower().Contains(qLower)) ||
                (p.ModoJuego != null && p.ModoJuego.ToLower().Contains(qLower)) ||
                (p.ClasificacionEdad != null && p.ClasificacionEdad.ToLower().Contains(qLower)) ||
                (p.CategoriasEspeciales != null && p.CategoriasEspeciales.ToLower().Contains(qLower))
            );
            ViewData["Busqueda"] = q;
        }
        if (novedades == true)
        {
            productos = productos.OrderByDescending(p => p.Id).Take(12);
            ViewData["Novedades"] = true;
        }
        else
        {
            productos = productos.OrderByDescending(p => p.Id);
        }
        var lista = productos.ToList();
        // Lógica de expiración de oferta
        var ahora = DateTime.UtcNow;
        foreach (var p in lista)
        {
            if (p.EsOferta && p.OfertaFin.HasValue && p.OfertaFin.Value < ahora)
            {
                p.EsOferta = false;
                // Solo reasignar si PrecioReal es válido
                if (p.PrecioReal.HasValue && p.PrecioReal.Value > 0)
                {
                    p.Precio = p.PrecioReal.Value;
                }
                // Si PrecioReal no es válido, mantener el precio actual (no poner a cero)
            }
            // Si el precio es cero o negativo, intentar corregir
            if (p.Precio <= 0)
            {
                if (p.PrecioReal.HasValue && p.PrecioReal.Value > 0)
                    p.Precio = p.PrecioReal.Value;
                else if (p.Descuento.HasValue && p.Descuento.Value > 0 && p.PrecioReal.HasValue && p.PrecioReal.Value > 0)
                    p.Precio = p.PrecioReal.Value - (p.PrecioReal.Value * p.Descuento.Value / 100);
                // Si sigue sin ser válido, dejar el precio como está (no poner a cero)
            }
        }
        return View(lista);
    }

    public IActionResult Producto(int id)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.Id == id);
        if (producto == null)
            return NotFound();
        // Lógica de expiración de oferta para producto individual
        var ahora = DateTime.UtcNow;
        if (producto.EsOferta && producto.OfertaFin.HasValue && producto.OfertaFin.Value < ahora)
        {
            producto.EsOferta = false;
            producto.Precio = producto.PrecioReal ?? producto.Precio;
        }
        // Cargar imágenes de galería ordenadas (base64 si existe, url si no)
        var imagenesGaleria = _context.ProductoImagenes
            .Where(i => i.ProductoId == id)
            .OrderBy(i => i.Orden)
            .ToList()
            .Select(i => i.ImagenData != null && i.ImagenMimeType != null
                ? $"data:{i.ImagenMimeType};base64,{Convert.ToBase64String(i.ImagenData)}"
                : i.Url)
            .ToList();
        ViewBag.ImagenesGaleria = (object)imagenesGaleria;
        return View(producto);
    }

    [HttpPost]
    public IActionResult AddToCart(int id, int cantidad)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                      (Request.Headers["Accept"].ToString().Contains("application/json"));
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Guardar la URL de retorno para después del login
            var returnUrl = Url.Action("Producto", "Home", new { id }) ?? "/";
            HttpContext.Session.SetString("ReturnToAfterLogin", returnUrl);
            if (isAjax)
            {
                return Json(new { success = false, loginRequired = true });
            }
            else
            {
                TempData["Error"] = "Primero debes iniciar sesión como cliente para poder añadir productos al carrito.";
                return Redirect($"/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
        }
        var carrito = HttpContext.Session.GetObjectFromJson<Dictionary<int, int>>("Carrito") ?? new Dictionary<int, int>();
        if (carrito.ContainsKey(id))
            carrito[id] += cantidad;
        else
            carrito[id] = cantidad;
        HttpContext.Session.SetObjectAsJson("Carrito", carrito);
        int cartCount = carrito.Values.Sum();
        return Json(new { success = true, cartCount });
    }

    public IActionResult Carrito()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
