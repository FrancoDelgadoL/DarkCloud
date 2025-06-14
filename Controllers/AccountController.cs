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
            // Si viene de intento de añadir al carrito, mostrar mensaje
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("/Home/Producto"))
            {
                TempData["Error"] = "Primero debes iniciar sesión como cliente para poder añadir productos al carrito.";
            }
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
                EmailConfirmed = true, // Permitir login inmediato
                Nombre = nombre,
                Apellido = apellido,
                Rol = "Cliente",
                FechaRegistro = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(usuario, password);
            if (result.Succeeded)
            {
                // Asegurar que el rol "Cliente" existe en Identity
                if (!await _userManager.IsInRoleAsync(usuario, "Cliente"))
                {
                    var roleManager = HttpContext.RequestServices.GetService(typeof(RoleManager<IdentityRole>)) as RoleManager<IdentityRole>;
                    if (roleManager != null && !await roleManager.RoleExistsAsync("Cliente"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Cliente"));
                    }
                    await _userManager.AddToRoleAsync(usuario, "Cliente");
                }
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
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null || string.IsNullOrEmpty(usuario.UserName))
            {
                TempData["Error"] = "Credenciales incorrectas.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(usuario.UserName, password, false, false);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Credenciales incorrectas.";
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }
            HttpContext.Session.SetString("UsuarioId", usuario.Id);
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
            HttpContext.Session.SetString("UsuarioRol", usuario.Rol);
            // Guardar nombre en TempData para el JS
            TempData["NombreUsuario"] = usuario.Nombre;
            // Guardar email en TempData para el JS
            TempData["EmailUsuario"] = usuario.Email;
            // Redirigir a login con guardacuenta=1 para activar prompt JS
            return Redirect($"/Account/Login?guardacuenta=1");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}