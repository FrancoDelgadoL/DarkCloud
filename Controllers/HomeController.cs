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
            productos = productos.Where(p =>
                (p.Nombre != null && p.Nombre.ToLower().Contains(q.ToLower())) ||
                (p.Descripcion != null && p.Descripcion.ToLower().Contains(q.ToLower())) ||
                (p.Genero != null && p.Genero.ToLower().Contains(q.ToLower())) ||
                (p.Plataforma != null && p.Plataforma.ToLower().Contains(q.ToLower())) ||
                (p.TipoProducto != null && p.TipoProducto.ToLower().Contains(q.ToLower())) ||
                (p.ModoJuego != null && p.ModoJuego.ToLower().Contains(q.ToLower())) ||
                (p.ClasificacionEdad != null && p.ClasificacionEdad.ToLower().Contains(q.ToLower())) ||
                (p.CategoriasEspeciales != null && p.CategoriasEspeciales.ToLower().Contains(q.ToLower()))
            );
            ViewData["Busqueda"] = q;
        }
        return View(productos.ToList());
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

    public IActionResult Catalogo(string? genero, string? plataforma, string? tipo, string? modo, string? clasificacion, string? categoria, bool? novedades)
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
        if (novedades == true)
        {
            productos = productos.OrderByDescending(p => p.Id).Take(12);
            ViewData["Novedades"] = true;
        }
        else
        {
            productos = productos.OrderByDescending(p => p.Id);
        }
        return View(productos.ToList());
    }

    public IActionResult Producto(int id)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.Id == id);
        if (producto == null)
            return NotFound();
        // Cargar imágenes de galería ordenadas
        var imagenesGaleria = _context.ProductoImagenes
            .Where(i => i.ProductoId == id)
            .OrderBy(i => i.Orden)
            .Select(i => i.Url)
            .ToList();
        ViewBag.ImagenesGaleria = imagenesGaleria;
        return View(producto);
    }

    [HttpPost]
    public IActionResult AddToCart(int id, int cantidad)
    {
        var usuarioId = HttpContext.Session.GetString("UsuarioId");
        if (string.IsNullOrEmpty(usuarioId))
        {
            // Guardar la URL de retorno para después del login
            var returnUrl = Url.Action("Producto", "Home", new { id }) ?? "/";
            HttpContext.Session.SetString("ReturnToAfterLogin", returnUrl);
            return Json(new { success = false, loginRequired = true });
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
