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
            // Procesar categorías especiales (multiselección)
            if (Request.Form["CategoriasEspeciales"].Count > 0)
            {
                producto.CategoriasEspeciales = string.Join(", ", Request.Form["CategoriasEspeciales"].ToArray());
            }
            // Guardar imagen principal en la base de datos
            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    ImagenArchivo.CopyTo(ms);
                    producto.ImagenPrincipal = ms.ToArray();
                    producto.ImagenPrincipalMimeType = ImagenArchivo.ContentType;
                }
            }
            // Procesar duración de la oferta
            if (producto.EsOferta && int.TryParse(Request.Form["DuracionOfertaHoras"], out int duracionHoras))
            {
                producto.DuracionOfertaHoras = duracionHoras;
                producto.OfertaInicio = DateTime.UtcNow;
                producto.OfertaFin = producto.OfertaInicio.Value.AddHours(duracionHoras);
            }
            else
            {
                producto.DuracionOfertaHoras = null;
                producto.OfertaInicio = null;
                producto.OfertaFin = null;
            }
            // Calcular precio de oferta si corresponde
            if (producto.EsOferta && producto.PrecioReal.HasValue && producto.Descuento.HasValue && producto.PrecioReal.Value > 0 && producto.Descuento.Value > 0)
            {
                producto.Precio = producto.PrecioReal.Value - (producto.PrecioReal.Value * producto.Descuento.Value / 100);
            }
            else
            {
                producto.Precio = producto.PrecioReal ?? 0;
                producto.Descuento = null;
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
                            using (var ms = new MemoryStream())
                            {
                                archivo.CopyTo(ms);
                                var img = new ProductoImagen
                                {
                                    ProductoId = producto.Id,
                                    ImagenData = ms.ToArray(),
                                    ImagenMimeType = archivo.ContentType,
                                    Orden = orden++
                                };
                                _context.ProductoImagenes.Add(img);
                            }
                        }
                    }
                    _context.SaveChanges();
                }
                return RedirectToAction("Productos");
            }
            if (producto.Imagenes == null)
                producto.Imagenes = new List<ProductoImagen>();
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
            // Guardar imagen principal en la base de datos
            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    ImagenArchivo.CopyTo(ms);
                    productoDb.ImagenPrincipal = ms.ToArray();
                    productoDb.ImagenPrincipalMimeType = ImagenArchivo.ContentType;
                }
            }

            // Eliminar imágenes de galería marcadas para eliminar
            if (EliminarGaleriaIds != null && EliminarGaleriaIds.Length > 0)
            {
                var imagenesEliminar = _context.ProductoImagenes.Where(img => EliminarGaleriaIds.Contains(img.Id)).ToList();
                _context.ProductoImagenes.RemoveRange(imagenesEliminar);
            }
            // Guardar nuevas imágenes de galería en la base de datos
            if (GaleriaArchivos != null && GaleriaArchivos.Count > 0)
            {
                int orden = productoDb.Imagenes?.Count() ?? 0;
                foreach (var archivo in GaleriaArchivos)
                {
                    if (archivo != null && archivo.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            archivo.CopyTo(ms);
                            var img = new ProductoImagen
                            {
                                ProductoId = productoDb.Id,
                                ImagenData = ms.ToArray(),
                                ImagenMimeType = archivo.ContentType,
                                Orden = orden++
                            };
                            _context.ProductoImagenes.Add(img);
                        }
                    }
                }
            }

            // Procesar duración de la oferta
            if (producto.EsOferta && int.TryParse(Request.Form["DuracionOfertaHoras"], out int duracionHoras))
            {
                productoDb.DuracionOfertaHoras = duracionHoras;
                productoDb.OfertaInicio = DateTime.UtcNow;
                productoDb.OfertaFin = productoDb.OfertaInicio.Value.AddHours(duracionHoras);
            }
            else
            {
                productoDb.DuracionOfertaHoras = null;
                productoDb.OfertaInicio = null;
                productoDb.OfertaFin = null;
            }

            // Calcular precio de oferta si corresponde (en edición)
            if (productoDb.EsOferta && productoDb.PrecioReal.HasValue && productoDb.Descuento.HasValue && productoDb.PrecioReal.Value > 0 && productoDb.Descuento.Value > 0)
            {
                productoDb.Precio = productoDb.PrecioReal.Value - (productoDb.PrecioReal.Value * productoDb.Descuento.Value / 100);
            }
            else
            {
                // Si no es oferta, el precio es igual al precio real
                productoDb.Precio = productoDb.PrecioReal ?? 0;
                productoDb.Descuento = null;
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
            var config = _context.HomeHeroConfigs.Include(h => h.ImagenesCarrusel).FirstOrDefault() ?? new Models.HomeHeroConfig {
                Titulo = "DARK CLOUD",
                FrasesJson = System.Text.Json.JsonSerializer.Serialize(new List<string> {
                    "¡Bienvenido a tu mundo gamer! \"Explora, juega y domina\"",
                    "\"Descubre los mejores títulos y ofertas\"",
                    "\"Tu aventura gamer comienza aquí\"",
                    "\"Juega, explora y conquista\""
                }),
                DuracionCarruselMs = 5000,
                TextoBoton = "Explorar Catálogo",
                EnlaceBoton = "#catalogo",
                MensajeBanner = "ENVÍO GRATIS EN PRODUCTOS SUPERIORES A S/99 | ENVÍOS A TODO PERÚ"
            };
            ViewBag.ImagenesCarrusel = config.ImagenesCarrusel
                .OrderBy(i => i.Orden)
                .Select(i => new {
                    Id = i.Id,
                    Base64 = $"data:{i.MimeType};base64,{Convert.ToBase64String(i.Imagen)}"
                }).ToList();
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
            // Imágenes existentes (IDs de imágenes que se mantienen)
            var imagenesExistentesIds = ImagenesExistentes?.Select(idStr => int.TryParse(idStr, out var id) ? id : 0).Where(id => id > 0).ToList() ?? new List<int>();

            // Eliminar imágenes que ya no están
            var imagenesActuales = _context.HomeHeroImagenes.Where(i => i.HomeHeroConfigId == config.Id).ToList();
            var imagenesAEliminar = imagenesActuales.Where(i => !imagenesExistentesIds.Contains(i.Id)).ToList();
            if (imagenesAEliminar.Any())
            {
                _context.HomeHeroImagenes.RemoveRange(imagenesAEliminar);
            }

            // Subida de nuevas imágenes
            if (ImagenesNuevas != null)
            {
                int ordenBase = imagenesActuales.Count;
                foreach (var img in ImagenesNuevas)
                {
                    if (img != null && img.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            img.CopyTo(ms);
                            var imagenBytes = ms.ToArray();
                            var imagenCarrusel = new HomeHeroImagen
                            {
                                Imagen = imagenBytes,
                                MimeType = img.ContentType,
                                HomeHeroConfig = config,
                                Orden = ordenBase++
                            };
                            _context.HomeHeroImagenes.Add(imagenCarrusel);
                        }
                    }
                }
            }
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

        [HttpGet]
        public async Task<IActionResult> EditarUsuario(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(string id, string Nombre, string Apellido, string Email, string Rol, string? NuevaPassword)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();
            usuario.Nombre = Nombre;
            usuario.Apellido = Apellido;
            usuario.Email = Email;
            usuario.UserName = Email;
            usuario.Rol = Rol;
            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return View(usuario);
            }
            if (!string.IsNullOrWhiteSpace(NuevaPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var passResult = await _userManager.ResetPasswordAsync(usuario, token, NuevaPassword);
                if (!passResult.Succeeded)
                {
                    TempData["Error"] = string.Join(", ", passResult.Errors.Select(e => e.Description));
                    return View(usuario);
                }
            }
            TempData["Mensaje"] = "Usuario actualizado correctamente.";
            return RedirectToAction("Usuarios");
        }
    }
}
