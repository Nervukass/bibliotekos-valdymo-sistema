using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private const string RoleSessionKey = "LibraryRole";
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewBag.Message = TempData["RoleMessage"];
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SelectRole(string role)
    {
        if (!LibraryRoles.All.Contains(role))
        {
            TempData["RoleMessage"] = "Pasirinkite vieną iš pateiktų rolių.";
            return RedirectToAction(nameof(Index));
        }

        HttpContext.Session.SetString(RoleSessionKey, role);
        return role switch
        {
            LibraryRoles.Librarian => RedirectToAction("Dashboard", "Library"),
            LibraryRoles.Administrator => RedirectToAction("Page", "Library", new { id = "admin-dashboard" }),
            _ => RedirectToAction("Page", "Library", new { id = "profile" })
        };
    }

    [HttpGet]
    public IActionResult Logout()
    {
        return HttpContext.Session.GetString(RoleSessionKey) is null
            ? RedirectToAction(nameof(Index))
            : View();
    }

    [HttpPost]
    [ActionName(nameof(Logout))]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmLogout()
    {
        HttpContext.Session.Remove(RoleSessionKey);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
