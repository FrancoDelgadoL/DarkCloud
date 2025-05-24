using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DarkCloud.Models;
using System.Security.Cryptography;
using System.Text;

namespace DarkCloud.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        public AccountController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        public async Task<IActionResult> Register(string nombre, string apellido, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
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
                Rol = "Cliente",
                FechaRegistro = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(usuario, password);
            if (result.Succeeded)
            {
                TempData["Mensaje"] = "Cuenta creada correctamente. Inicia sesión.";
                return RedirectToAction("Login");
            }
            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Correo y contraseña requeridos.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Credenciales incorrectas.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario != null)
            {
                HttpContext.Session.SetString("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                HttpContext.Session.SetString("UsuarioRol", usuario.Rol);
            }
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
            if (usuario != null)
            {
                return RedirectToAction(usuario.Rol == "Administrador" ? "Index" : "Index", usuario.Rol == "Administrador" ? "Administrador" : "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}