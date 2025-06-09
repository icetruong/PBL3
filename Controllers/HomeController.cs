using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HoldEvent.Models;
using HoldEvent.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

        var currentTime = DateTime.Now;
        var pastEvent = await _DbContext.Events.Where(e => e.StartTime < currentTime && e.EventStatus != 2).ToListAsync();
        foreach (var e in pastEvent)
        {
            e.EventStatus = 2;
        }
        await _DbContext.SaveChangesAsync();


        List<Event> le = await _DbContext.Events.Where(e => e.EventStatus == 1).ToListAsync();

        if (searchName != null)
            le =  le.Where(e => e.Name == searchName).ToList();
        var random = new Random();
        var events = le.OrderBy(e => random.Next()).ToList();
        return View(events);
    }

    public async Task<IActionResult> IndexAdmin()
    {
        var AdminID = HttpContext.Session.GetString("UserId");
        var RoleAdmin = HttpContext.Session.GetString("Role");
        var FullName = HttpContext.Session.GetString("FullName");

        ViewData["UserId"] = AdminID;
        ViewData["Role"] = RoleAdmin;
        ViewData["FullName"] = FullName;

        var upcomingEvents = new List<EventInfo>();
        
        var le = await _DbContext.Events.Where(e => e.StartTime > DateTime.Now && e.EventStatus == 1).OrderBy(e => e.StartTime).Take(20).ToListAsync();
        foreach(var e in le)
        {
            var tickets = await _DbContext.Tickets.Where(p => p.EventId == e.EventId).ToListAsync();
            var v = await _DbContext.Venues.SingleOrDefaultAsync(p => p.VenueId == e.VenueId);
            upcomingEvents.Add(new EventInfo
            {
                Name = e.Name,
                StartTime = (DateTime)e.StartTime,
                EndTime = (DateTime)e.EndTime,
                VenueName = v.Name,
                TotalQuantity = tickets.Sum(t => t.TotalQuantity) ?? 0,
                SoldQuantity = tickets.Sum(t => t.SoldQuantity) ?? 0
            });

        }
        return View(new AdminViewModel
        {
            TotalEvents = await _DbContext.Events.CountAsync(),
            TotalUsers = await _DbContext.Users.CountAsync(),
            TotalTicketsSold = await _DbContext.Tickets.SumAsync(t => t.SoldQuantity) ?? 0,
            UpcomingEvents = upcomingEvents,
        });


    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
