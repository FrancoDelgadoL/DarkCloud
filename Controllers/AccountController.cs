using Microsoft.AspNetCore.Mvc;

namespace DarkCloud.Controllers
{
    public class AccountController : Controller
    {
        // Acción para mostrar la página de inicio de sesión
        public IActionResult Login()
        {
            return View();
        }

        // Acción para mostrar la página de registro
        public IActionResult Register()
        {
            return View();
        }
    }
}