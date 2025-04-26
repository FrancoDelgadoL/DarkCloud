using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DarkCloud.Models;

namespace DarkCloud.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        ViewData["OcultarFooter"] = true;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(string name, string email, string message)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
        {
            TempData["Error"] = "Todos los campos son obligatorios.";
            return RedirectToAction("Index");
        }

        // Aquí podrías agregar lógica para enviar el mensaje, como guardarlo en una base de datos o enviarlo por correo electrónico.
        TempData["Success"] = "Gracias por contactarnos. Nos pondremos en contacto contigo pronto.";
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
