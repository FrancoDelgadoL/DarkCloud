using Microsoft.AspNetCore.Mvc;
using DarkCloud.Models;
using System.Security.Cryptography;
using System.Text;

namespace DarkCloud.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para mostrar la página de inicio de sesión
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Acción para mostrar la página de registro
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string nombre, string apellido, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Todos los campos son obligatorios.";
                return View();
            }
            if (_context.Usuarios.Any(u => u.Email == email))
            {
                TempData["Error"] = "El correo ya está registrado.";
                return View();
            }
            var usuario = new Usuario
            {
                Nombre = nombre,
                Apellido = apellido,
                Email = email,
                PasswordHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password))),
                Rol = "Cliente",
                FechaRegistro = DateTime.UtcNow
            };
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            TempData["Mensaje"] = "Cuenta creada correctamente. Inicia sesión.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Correo y contraseña requeridos.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);
            if (usuario == null)
            {
                TempData["Error"] = "Credenciales incorrectas.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            var returnTo = HttpContext.Session.GetString("ReturnToAfterLogin");
            if (!string.IsNullOrEmpty(returnTo))
            {
                HttpContext.Session.Remove("ReturnToAfterLogin");
                return Redirect(returnTo);
            }
            return RedirectToAction(usuario.Rol == "Administrador" ? "Index" : "Index", usuario.Rol == "Administrador" ? "Administrador" : "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}