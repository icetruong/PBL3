using AspNetCoreGeneratedDocument;
using HoldEvent.Models;
using HoldEvent.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoldEvent.Controllers
{
    public class AdminController : Controller
    {
        private readonly Pbl3Context _DbContext;

        public AdminController(Pbl3Context DbContext)
        {
            _DbContext = DbContext;
        }
        public IActionResult IndexAdmin()
        {
            return RedirectToAction("IndexAdmin", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ListSupport()
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var ls = await _DbContext.Feedbacks.Where(p => p.FeedbackState == 0).ToListAsync();

            return View(ls);
        }

        [HttpGet]
        public async Task<IActionResult> Support(String FeedbackID)
        {
            var userID = HttpContext.Session.GetString("UserId");
            var admin = await _DbContext.Admins.SingleOrDefaultAsync(p => p.UserId == userID);
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var feedback = await _DbContext.Feedbacks.FindAsync(FeedbackID);
            return View(new FeedbackSupportViewModel
            {
                SupportId = await getVaildIDBySupport(),
                FeedbackId = feedback.FeedbackId,
                UserId = feedback.UserId,
                AdminId = admin.AdminId,
                Title = feedback.Title,
                ContentFeedback = feedback.Content,
                CreateAtDayFeedback = feedback.CreateAtDay,
                ContentSupport = null,
                CreateAtDaySupport = null,
            });
        }
        [HttpPost]

        public async Task<IActionResult> Support(FeedbackSupportViewModel model)
        {
            if (String.IsNullOrWhiteSpace(model.ContentSupport))
                ModelState.AddModelError("ContentSupport", "Content Support khong duoc de trong");
            if (!ModelState.IsValid)
                return View(model);

            var support = new Support
            {
                SupportId = model.SupportId,
                AdminId = model.AdminId,
                FeedbackId = model.FeedbackId,
                Content = model.ContentSupport,
                CreateAtDay = DateTime.Now
            };

            var originalF = await _DbContext.Feedbacks.FindAsync(model.FeedbackId);
            originalF.FeedbackState = 1;

            await _DbContext.Supports.AddAsync(support);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("ListSupport", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> ViewCommission()
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var commissionTicket = await _DbContext.Commissions.OrderBy(id => id).LastOrDefaultAsync(p => p.CommissionType == 1);
            var commissionVenue =await _DbContext.Commissions.OrderBy(id => id).LastOrDefaultAsync(p => p.CommissionType == 2);

            var commissions = new List<Commission>();
            if (commissionTicket != null)
                commissions.Add(commissionTicket);
            if (commissionVenue != null)
                commissions.Add(commissionVenue);
            return View(commissions);
        }
        [HttpGet]
        public async Task<IActionResult> EditCommission(int Type)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var commision = await _DbContext.Commissions.OrderBy(id => id).LastOrDefaultAsync(p => p.CommissionType == Type);
            return View(commision);
        }

        [HttpPost]
        public async Task<IActionResult> EditCommission(Commission model)
        {
            if (model.Percentage < 0 && model.Percentage > 100)
                ModelState.AddModelError("Percentage", "Percentage phai nam trong khoang tu 0->100");
            if (!ModelState.IsValid)
                return View(model);
            var commission = new Commission
            {
                CommissionId = await getVaildIDByCommission(),
                CommissionType = model.CommissionType,
                Percentage = model.Percentage,
                CreateAtDay = DateTime.Now
            };

            await _DbContext.Commissions.AddAsync(commission);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("ViewCommission", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> ListUser(String? searchName)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            List<User> lu;
            if (searchName == null)
            {
                lu = await _DbContext.Users.Where(p => p.Role != "Admin").ToListAsync();
            }
            else
            {
                lu = await _DbContext.Users.Where(p => p.Role != "Admin" && p.FullName.StartsWith(searchName)).ToListAsync();
                ViewData["Search"] = searchName;
            }    

            return View(lu);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(String UserID)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var u = await _DbContext.Users.SingleOrDefaultAsync(p => p.UserId == UserID);
            return View(u);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(User model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var u = await _DbContext.Users.FindAsync(model.UserId);
            u.Role = model.Role;
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("ListUser", "Admin");
        }

        [HttpGet]

        public async Task<IActionResult> ListEvent()
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            List<Event> le = await _DbContext.Events.Where(e => e.EventStatus == -1).ToListAsync();

            return View(le);
        }

        [HttpGet]

        public async Task<IActionResult> InspectDetailEvent(String EventID)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            var e = await _DbContext.Events.FindAsync(EventID);
            return View(e);
        }

        [HttpPost]
        public async Task<IActionResult> InspectDetailEvent(Event model, String EventStatus)
        {
            var e = await _DbContext.Events.FindAsync(model.EventId);
            if (EventStatus == "Approved")
                e.EventStatus = 1;
            else
                e.EventStatus = 0;
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("ListEvent", "Admin");

        }

        [HttpGet]

        public async Task<IActionResult> RevenueAdmin(String typeTime, DateTime? date, int? month, String typeSource)
        {
            var AdminID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            if (typeTime == null)
            {
                typeTime = "total";
            }
            if (typeSource == null)
            {
                typeSource = "all";
            }


            ViewData["TypeTime"] = typeTime;
            ViewData["TypeSource"] = typeSource;

            if(typeTime == "date" && date == null)
            {
                return View();
            }
            if (typeTime == "month" && month == null)
            {
                return View();
            }



            List<RevenueReportAdminViewModel> lvr = new List<RevenueReportAdminViewModel>();
            if(typeSource == "venue" || typeSource == "all")
            {
                List<Booking> lb = await _DbContext.Bookings.Where(b => b.BookingStatus == 1).Include(b => b.Venue).ThenInclude(v => v.Commission).ToListAsync();
                if (typeTime == "date" && date != null)
                {
                    lb = lb.Where(b => b.BookingDate != null && b.BookingDate.Value.Date == date.Value.Date && b.BookingDate.Value.Month == date.Value.Month && b.BookingDate.Value.Year == date.Value.Year).ToList();
                }
                if (typeTime == "month" && month != null)
                {
                    lb = lb.Where(b => b.BookingDate != null && b.BookingDate.Value.Month == month).ToList();
                }
                var lvrVenue = lb.GroupBy(b => new { b.Venue.VenueId, b.Venue.Name, Commission = b.Venue.Commission.Percentage ?? 0 })
                    .Select(g => new RevenueReportAdminViewModel
                    {
                        SourceName = g.Key.Name,
                        Type = "Địa điểm",
                        CommissionPercent = g.Key.Commission,
                        TotalRevenue = g.Sum(b => b.Deposit ?? 0),
                        CommissionEarned = g.Sum(b => b.Deposit ?? 0) * ((decimal)g.Key.Commission / 100)
                    }).ToList();

                lvr.AddRange(lvrVenue);
            }
            if (typeSource == "event" || typeSource == "all")
            {
                List<Transaction> lt = await _DbContext.Transactions.Include(t => t.Ticket).ThenInclude(t => t.Commission).Include(t => t.Ticket).ThenInclude(t => t.Event).Include(t => t.Payment).ToListAsync();
                if (typeTime == "date" && date != null)
                {
                    lt = lt.Where(t => t.TransactionDate != null && t.TransactionDate.Value.Date == date.Value.Date && t.TransactionDate.Value.Month == date.Value.Month && t.TransactionDate.Value.Year == date.Value.Year).ToList();
                }
                if (typeTime == "month" && date != null)
                {
                    lt = lt.Where(t => t.TransactionDate != null && t.TransactionDate.Value.Month == month).ToList();
                }
                var lvrEvent = lt.GroupBy(t => new { t.Ticket.Event.EventId, t.Ticket.Event.Name, Commission = t.Ticket.Commission.Percentage ?? 0 })
                    .Select(g => new RevenueReportAdminViewModel
                    {
                        SourceName = g.Key.Name,
                        Type = "Sự kiện",
                        CommissionPercent = g.Key.Commission,
                        TotalRevenue = g.Sum(tr => tr.Payment.Price ?? 0),
                        CommissionEarned = g.Sum(tr => (tr.Payment.Price ?? 0) * ((decimal)g.Key.Commission / 100))
                    }).ToList();
                lvr.AddRange(lvrEvent);
            }
            ViewData["TotalCommission"] = lvr.Sum(r => r.CommissionEarned);

            return View(lvr);
        }

        private async Task<String> getVaildIDBySupport()
        {
            var lID = await _DbContext.Supports.Select(p => p.SupportId).OrderBy(id => id).ToListAsync();
            int x = 1;
            foreach (var a in lID)
            {
                if (Convert.ToInt32(a) != x)
                    break;
                x++;
            }

            return x.ToString("D8");
        }

        private async Task<String> getVaildIDByCommission()
        {
            var lID = await _DbContext.Commissions.Select(p => p.CommissionId).OrderBy(id => id).ToListAsync();
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
