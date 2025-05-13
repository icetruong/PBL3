using HoldEvent.Models;
using HoldEvent.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoldEvent.Controllers
{
    public class OwnPlaceController : Controller
    {
        private readonly Pbl3Context _DbContext;

        public OwnPlaceController(Pbl3Context DbContext)
        {
            _DbContext = DbContext;
        }

        public async Task<IActionResult> ListVenue(String? searchName)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var OwnPlaceID = HttpContext.Session.GetString("UserId");
            List<Venue> lv;
            if (searchName == null)
                lv = await _DbContext.Venues.Where(p => p.OwnPlaceId == OwnPlaceID).ToListAsync();
            else
                lv = await _DbContext.Venues.Where(p => p.OwnPlaceId == OwnPlaceID && p.Name.StartsWith(searchName)).ToListAsync();
            ViewData["Search"] = searchName;
            return View(lv);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEditVenue(String? VenueID)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            // Create
            if (VenueID == null)
            {
                return View(new Venue
                {
                    VenueId = await getVaildIDByVenue(),
                    OwnPlaceId = null,
                    CommissionId = null,
                    Name = "",
                    Description = null,
                    Capacity = null,
                    Address = null,
                    Price = null
                });
            }   
            // Edit
            else
            {
                var v = await _DbContext.Venues.SingleOrDefaultAsync(p => p.VenueId == VenueID);
                return View(v);
            }    
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditVenue(Venue model)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var OwnPlaceID = HttpContext.Session.GetString("UserId");
            var commission = await _DbContext.Commissions.OrderBy(id => id).LastOrDefaultAsync(p => p.CommissionType == 2);
            if (model.Capacity <= 0)
                ModelState.AddModelError("Capacity", "Capacity lon hon 0");
            if (model.Price <= 0)
                ModelState.AddModelError("Price", "Price lon hon 0");
            if (!ModelState.IsValid)
                return View(model);

            // Create
            if(model.OwnPlaceId == null)
            {
                var newVenue = new Venue
                {
                    VenueId = model.VenueId,
                    OwnPlaceId = OwnPlaceID,
                    CommissionId = commission.CommissionId,
                    Name = model.Name,
                    Description = model.Description,
                    Capacity = model.Capacity,
                    Address = model.Address,
                    Price = model.Price
                };
                await _DbContext.Venues.AddAsync(newVenue);

            }
            // Edit
            else
            {
                var original = await _DbContext.Venues.FindAsync(model.VenueId);
                original.Name = model.Name;
                original.Description = model.Description;
                original.Capacity = model.Capacity;
                original.Address = model.Address;
                original.Price = model.Price;
            }
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("ListVenue", "OwnPlace");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteVenue(String VenueID)
        {
            var bookingVenue = await _DbContext.Bookings.SingleOrDefaultAsync(p => p.VenueId == VenueID);
            if (bookingVenue != null)
            {
                ModelState.AddModelError("Name", "Khong the xoa venue khi dang co nguoi dat");
                return RedirectToAction("ListVenue", "OwnPlace");
            }    
            var venue = await _DbContext.Venues.SingleOrDefaultAsync(p => p.VenueId == VenueID);
            _DbContext.Venues.Remove(venue);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("ListVenue", "OwnPlace");
        }

        [HttpGet]
        public async Task<IActionResult> ListBooking()
        {
            var OwnPlaceID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            List<Booking> lb = new List<Booking>();
            var venue = await _DbContext.Venues.Where(p => p.OwnPlaceId == OwnPlaceID).ToListAsync();
            foreach (var v in venue)
            {

                var bookings = await _DbContext.Bookings.Where(p => p.VenueId == v.VenueId).ToListAsync();

                foreach (var b in bookings)
                {
                    lb.Add(b);
                }
            }

            return View(lb);
        }
        [HttpGet]
        public async Task<IActionResult> InspectBooking(String BookingID)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            var b = await _DbContext.Bookings.SingleOrDefaultAsync(p => p.BookingId == BookingID);

            return View(b);
        }

        [HttpPost]
        public async Task<IActionResult> InspectBooking(Booking model, String BookingStatus)
        {
            var e = await _DbContext.Bookings.SingleOrDefaultAsync(p => p.BookingId == model.BookingId);
            if (BookingStatus == "Approved")
                e.BookingStatus = 1;
            else
                e.BookingStatus = 0;

            await _DbContext.SaveChangesAsync();
            return RedirectToAction("ListBooking", "OwnPlace");
        }

        [HttpGet]
        public async Task<IActionResult> RevenueOwnPlace(String type, DateTime? date, int? month)
        {
            var OwnPlaceID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            ViewData["Type"] = type;

            List<Booking> lb = new List<Booking>();
            var venue = await _DbContext.Venues.Where(v => v.OwnPlaceId == OwnPlaceID).Include(v => v.Commission).ToListAsync();
            foreach (var v in venue)
            {
                var bookings = await _DbContext.Bookings.Where(b => b.VenueId == v.VenueId && b.BookingStatus == 1).Include(b => b.Venue).ToListAsync();

                lb.AddRange(bookings);
            }
            if (type == "date" && date != null)
            {
                lb = lb.Where(b => b.BookingDate != null && b.BookingDate.Value.Date == date.Value.Date && b.BookingDate.Value.Month == date.Value.Month && b.BookingDate.Value.Year == date.Value.Year).ToList();  
            }
            if (type == "month" && month != null)
            {
                lb = lb.Where(b => b.BookingDate != null && b.BookingDate.Value.Month == month).ToList();
            }

            List<RevenueReportVenueViewModel> lvr = new List<RevenueReportVenueViewModel>();

            if(type == "venue")
            {
                lvr = lb.GroupBy(b => new { b.Venue.VenueId, b.Venue.Name, Commission = b.Venue.Commission.Percentage ?? 0 })
                    .Select(g => new RevenueReportVenueViewModel
                    {
                        VenueName = g.Key.Name,
                        BookingCount = g.Count(),
                        TotalRevenue = g.Sum(b => b.Deposit ?? 0),
                        NetRevenue = g.Sum(b => b.Deposit ?? 0) * (1 - (decimal)g.Key.Commission / 100)
                    }).ToList();
            }

            ViewData["TotalOverallRevenue"] = lb.Sum(b => b.Deposit ?? 0);
            ViewData["TotalNetOverallRevenue"] = lb.Sum(b =>
            {
                var commission = b.Venue.Commission.Percentage ?? 0;
                return Math.Round((b.Deposit ?? 0) * (1 - (decimal)commission / 100), 2);
                
            });
            return View(lvr);
        }

        private async Task<String> getVaildIDByVenue()
        {
            var lID = await _DbContext.Venues.Select(p => p.VenueId).OrderBy(id => id).ToListAsync();
            int x = 1;
            foreach (var a in lID)
            {
                if (Convert.ToInt32(a) != x)
                    break;
                x++;
            }

            return x.ToString("D8");
        }
    }
}
