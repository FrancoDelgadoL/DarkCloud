using Microsoft.AspNetCore.Mvc;
using DarkCloud.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace DarkCloud.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        public AdministradorController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var productos = _context.Productos.ToList();
            return View(productos);
        }

        public IActionResult Productos()
        {
            var productos = _context.Productos.ToList();
            return View(productos);
        }

        [HttpGet]
        public IActionResult AgregarProducto()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarProducto(Producto producto, IFormFile ImagenArchivo, List<IFormFile> GaleriaArchivos)
        {
            // Asegurar que la carpeta wwwroot/images existe
            var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }
            // Procesar categorías especiales (multiselección)
            if (Request.Form["CategoriasEspeciales"].Count > 0)
            {
                producto.CategoriasEspeciales = string.Join(", ", Request.Form["CategoriasEspeciales"].ToArray());
            }
            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                var nombreArchivo = Path.GetFileNameWithoutExtension(ImagenArchivo.FileName);
                var extension = Path.GetExtension(ImagenArchivo.FileName);
                var nombreUnico = $"{nombreArchivo}_{Guid.NewGuid()}{extension}";
                var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", nombreUnico);
                using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                {
                    ImagenArchivo.CopyTo(stream);
                }
                producto.ImagenUrl = $"/images/{nombreUnico}";
            }
            if (ModelState.IsValid)
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                // Guardar imágenes de galería
                if (GaleriaArchivos != null && GaleriaArchivos.Count > 0)
                {
                    int orden = 0;
                    foreach (var archivo in GaleriaArchivos)
                    {
                        if (archivo != null && archivo.Length > 0)
                        {
                            var nombreArchivo = Path.GetFileNameWithoutExtension(archivo.FileName);
                            var extension = Path.GetExtension(archivo.FileName);
                            var nombreUnico = $"galeria_{Guid.NewGuid()}{extension}";
                            var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", nombreUnico);
                            using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                            {
                                archivo.CopyTo(stream);
                            }
                            var img = new ProductoImagen
                            {
                                ProductoId = producto.Id,
                                Url = $"/images/{nombreUnico}",
                                Orden = orden++
                            };
                            _context.ProductoImagenes.Add(img);
                        }
                    }
                    _context.SaveChanges();
                }
                return RedirectToAction("Productos");
            }
            return View(producto);
        }

        public IActionResult EditarProducto(int id)
        {
            var producto = _context.Productos
                .Include(p => p.Imagenes)
                .FirstOrDefault(p => p.Id == id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarProducto(Producto producto, IFormFile ImagenArchivo, List<IFormFile> GaleriaArchivos, int[] EliminarGaleriaIds)
        {
            var productoDb = _context.Productos.FirstOrDefault(p => p.Id == producto.Id);
            if (productoDb == null) return NotFound();

            // Actualizar campos
            productoDb.Nombre = producto.Nombre;
            productoDb.Descripcion = producto.Descripcion;
            productoDb.Precio = producto.Precio;
            productoDb.PrecioReal = producto.PrecioReal;
            productoDb.Descuento = producto.Descuento;
            productoDb.EsOferta = producto.EsOferta;
            productoDb.Genero = producto.Genero;
            productoDb.Plataforma = producto.Plataforma;
            productoDb.TipoProducto = producto.TipoProducto;
            productoDb.ModoJuego = producto.ModoJuego;
            productoDb.ClasificacionEdad = producto.ClasificacionEdad;
            productoDb.CategoriasEspeciales = string.Join(", ", Request.Form["CategoriasEspeciales"].ToArray());

            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                var nombreArchivo = Path.GetFileNameWithoutExtension(ImagenArchivo.FileName);
                var extension = Path.GetExtension(ImagenArchivo.FileName);
                var nombreUnico = $"{nombreArchivo}_{Guid.NewGuid()}{extension}";
                var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", nombreUnico);
                using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                {
                    ImagenArchivo.CopyTo(stream);
                }
                productoDb.ImagenUrl = $"/images/{nombreUnico}";
            }

            // Eliminar imágenes de galería marcadas
            if (EliminarGaleriaIds != null && EliminarGaleriaIds.Length > 0)
            {
                var imagenesEliminar = _context.ProductoImagenes.Where(i => EliminarGaleriaIds.Contains(i.Id) && i.ProductoId == productoDb.Id).ToList();
                _context.ProductoImagenes.RemoveRange(imagenesEliminar);
            }

            // Agregar nuevas imágenes de galería
            if (GaleriaArchivos != null && GaleriaArchivos.Count > 0)
            {
                int orden = productoDb.Imagenes?.Count() ?? 0;
                foreach (var archivo in GaleriaArchivos)
                {
                    if (archivo != null && archivo.Length > 0)
                    {
                        var nombreArchivo = Path.GetFileNameWithoutExtension(archivo.FileName);
                        var extension = Path.GetExtension(archivo.FileName);
                        var nombreUnico = $"galeria_{Guid.NewGuid()}{extension}";
                        var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", nombreUnico);
                        using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                        {
                            archivo.CopyTo(stream);
                        }
                        var img = new ProductoImagen
                        {
                            ProductoId = productoDb.Id,
                            Url = $"/images/{nombreUnico}",
                            Orden = orden++
                        };
                        _context.ProductoImagenes.Add(img);
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Productos");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarProducto(int id)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.Id == id);
            if (producto == null) return NotFound();
            _context.Productos.Remove(producto);
            _context.SaveChanges();
            return RedirectToAction("Productos");
        }

        [HttpGet]
        public IActionResult HomeHeroConfig()
        {
            var config = _context.HomeHeroConfigs.FirstOrDefault() ?? new Models.HomeHeroConfig {
                Titulo = "DARK CLOUD",
                FrasesJson = System.Text.Json.JsonSerializer.Serialize(new List<string> {
                    "¡Bienvenido a tu mundo gamer! \"Explora, juega y domina\"",
                    "\"Descubre los mejores títulos y ofertas\"",
                    "\"Tu aventura gamer comienza aquí\"",
                    "\"Juega, explora y conquista\""
                }),
                ImagenesCarruselJson = System.Text.Json.JsonSerializer.Serialize(new List<string> {
                    "/images/carru1.jpg",
                    "/images/carru2.jpe",
                    "/images/carru3.jpg"
                }),
                DuracionCarruselMs = 5000,
                TextoBoton = "Explorar Catálogo",
                EnlaceBoton = "#catalogo",
                MensajeBanner = "ENVÍO GRATIS EN PRODUCTOS SUPERIORES A S/99 | ENVÍOS A TODO PERÚ"
            };
            return View(config);
        }

        [HttpPost]
        public IActionResult HomeHeroConfig(string? Titulo, string[]? Frases, string[]? ImagenesExistentes, int? DuracionCarruselSeg, string? TextoBoton, string? EnlaceBoton, string? MensajeBanner, List<IFormFile>? ImagenesNuevas)
        {
            var config = _context.HomeHeroConfigs.FirstOrDefault();
            if (config == null)
            {
                config = new Models.HomeHeroConfig();
                _context.HomeHeroConfigs.Add(config);
            }
            config.Titulo = Titulo;
            // Frases
            var frasesList = Frases?.Where(f => !string.IsNullOrWhiteSpace(f)).ToList() ?? new List<string>();
            config.FrasesJson = System.Text.Json.JsonSerializer.Serialize(frasesList);
            // Imágenes existentes
            var imagenes = ImagenesExistentes?.ToList() ?? new List<string>();
            // Subida de nuevas imágenes
            if (ImagenesNuevas != null)
            {
                foreach (var img in ImagenesNuevas)
                {
                    if (img != null && img.Length > 0)
                    {
                        var nombreArchivo = Path.GetFileNameWithoutExtension(img.FileName);
                        var extension = Path.GetExtension(img.FileName);
                        var nombreUnico = $"carrusel_{Guid.NewGuid()}{extension}";
                        var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", nombreUnico);
                        using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                        {
                            img.CopyTo(stream);
                        }
                        imagenes.Add($"/images/{nombreUnico}");
                    }
                }
            }
            config.ImagenesCarruselJson = System.Text.Json.JsonSerializer.Serialize(imagenes);
            // Validar duración
            int duracionMs = (DuracionCarruselSeg.HasValue && DuracionCarruselSeg.Value > 0) ? DuracionCarruselSeg.Value * 1000 : 5000;
            config.DuracionCarruselMs = duracionMs;
            config.TextoBoton = TextoBoton;
            config.EnlaceBoton = EnlaceBoton;
            config.MensajeBanner = MensajeBanner;
            _context.SaveChanges();
            TempData["Mensaje"] = "Configuración guardada correctamente.";
            return RedirectToAction("HomeHeroConfig");
        }

        [HttpGet]
        public IActionResult Usuarios()
        {
            var usuarios = _userManager.Users.ToList();
            return View(usuarios);
        }

        [HttpGet]
        public IActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario(string nombre, string apellido, string email, string password, string rol)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(rol))
            {
                TempData["Error"] = "Todos los campos son obligatorios.";
                return View();
            }
            var existe = await _userManager.FindByEmailAsync(email);
            if (existe != null)
            {
                TempData["Error"] = "El correo ya está registrado.";
                return View();
            }
            var usuario = new Usuario
            {
                UserName = email,
                Email = email,
                Nombre = nombre,
                Apellido = apellido,
                Rol = rol,
                FechaRegistro = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(usuario, password);
            if (result.Succeeded)
            {
                TempData["Mensaje"] = "Usuario creado correctamente.";
                return RedirectToAction("Usuarios");
            }
            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }
    }
}
