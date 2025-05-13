using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HoldEvent.Models;
using HoldEvent.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HoldEvent.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    protected readonly Pbl3Context _DbContext;

    public HomeController(ILogger<HomeController> logger, Pbl3Context dbContext)
    {
        _logger = logger;
        _DbContext = dbContext;
    }

    public async Task<IActionResult> Index(String? searchName)
    {
        var UserID = HttpContext.Session.GetString("UserId");
        var Role = HttpContext.Session.GetString("Role");
        var FullName = HttpContext.Session.GetString("FullName");

        ViewData["UserId"] = UserID;
        ViewData["Role"] = Role;
        ViewData["FullName"] = FullName;

        List<Event> le;
        if (searchName == null)
            le = await _DbContext.Events.Where(e => e.EventStatus == 1).ToListAsync();
        else
            le = await _DbContext.Events.Where(e => e.Name == searchName && e.EventStatus == 1).ToListAsync();
        return View(le);
    }

    public IActionResult IndexAdmin()
    {
        var AdminID = HttpContext.Session.GetString("UserId");
        var RoleAdmin = HttpContext.Session.GetString("Role");
        var FullName = HttpContext.Session.GetString("FullName");

        ViewData["UserId"] = AdminID;
        ViewData["Role"] = RoleAdmin;
        ViewData["FullName"] = FullName;
        return View();


    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
