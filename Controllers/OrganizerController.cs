using HoldEvent.Models;
using HoldEvent.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace HoldEvent.Controllers
{
    public class OrganizerController : Controller
    {
        private readonly Pbl3Context _DbContext;

        public OrganizerController(Pbl3Context DbContext)
        {
            _DbContext = DbContext;
        }

        [HttpGet]
        public async Task<IActionResult> ListVenue(VenueSearchViewModel model)
        {
            var OrganizerID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            if (string.IsNullOrWhiteSpace(model.Select))
                model.Select = "name";

            
            if (string.IsNullOrWhiteSpace(model.Search) && model.Select != "name")
            {
                ModelState.AddModelError("Search", "Search không được để trống");
                model.Venues = await _DbContext.Venues.ToListAsync();
                return View(model);
            }
            switch(model.Select)
            {
                case "name":
                    if (String.IsNullOrWhiteSpace(model.Search))
                        model.Venues = await _DbContext.Venues.ToListAsync();
                    else
                        model.Venues = await _DbContext.Venues.Where(p => p.Name.StartsWith(model.Search)).ToListAsync();
                    break;
                case "capacity":
                    foreach (char c in model.Search)
                    {
                        if (!char.IsDigit(c))
                        {
                            ModelState.AddModelError("Search", "Select = capacity thì không được nhập chữ");
                            model.Venues = await _DbContext.Venues.ToListAsync();
                            return View(model);
                        }      
                    }
                    model.Venues = await _DbContext.Venues.Where(p => p.Capacity == Convert.ToInt32(model.Search)).ToListAsync();
                    break;
                case "price":
                    foreach (char c in model.Search)
                    {
                        if (!char.IsDigit(c))
                        {
                            if (c != '.')
                            {
                                ModelState.AddModelError("Search", "Select = price thì không được nhập chữ");
                                model.Venues = await _DbContext.Venues.ToListAsync();
                                return View(model);
                            }
                        }
                    }
                    model.Venues = await _DbContext.Venues.Where(p => p.Price <= Convert.ToDecimal(model.Search)).ToListAsync();
                    break;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BookingVenue(String VenueID)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var venue = await _DbContext.Venues.FindAsync(VenueID);
            return View(new VenueBookingViewModel
            {
                VenueId = venue.VenueId,
                Name = venue.Name,
                Description = venue.Description,
                Capacity = venue.Capacity,
                Address = venue.Address,
                Price = venue.Price,
                DateStart = null,
                DateEnd = null,
                Deposit = 0,
                PaymentId = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> BookingVenue(VenueBookingViewModel model)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            if (model.DateStart == null)
                ModelState.AddModelError("DateStart", "Yeu cau nhap Date Start");
            if (model.DateEnd == null)
                ModelState.AddModelError("DateEnd", "Yeu cau nhap Date End");
            if (!ModelState.IsValid)
                return View(model);
            if (model.DateEnd < model.DateStart && model.DateStart > DateTime.Now)
                ModelState.AddModelError("DateEnd", "Loi thoi gian khong hop le");
            DateTime dateStart = (DateTime)model.DateStart;
            DateTime dateEnd = (DateTime)model.DateEnd;

            if (!await checkBookingConflictByVenue(dateStart, dateEnd, model.VenueId))
                ModelState.AddModelError("DateEnd", "Loi thoi gian nay da co nguoi dat dia diem");

            if (!ModelState.IsValid)
                return View(model);

            Decimal days =(Decimal) (dateEnd - dateStart).TotalDays;
            Decimal TotalMoney = days * (decimal)model.Price;

            ModelState.Remove("Deposit");
            model.Deposit = Math.Round(TotalMoney,2);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentVenue(String VenueID, DateTime? DateStart, DateTime? DateEnd, Decimal? Deposit)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            var venue = await _DbContext.Venues.FindAsync(VenueID);
            return View(new VenueBookingViewModel
            {
                VenueId = venue.VenueId,
                Name = venue.Name,
                Description = venue.Description,
                Capacity = venue.Capacity,
                Address = venue.Address,
                Price = venue.Price,
                DateStart = DateStart,
                DateEnd = DateEnd,
                Deposit = Deposit,
                PaymentId = null,
                Method = 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> PaymentVenue(VenueBookingViewModel model)
        {

            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            var paymentID = await getVaildIDByPayment();
            if (model.PricePayment == null)
            {
                if (model.Method == 1 && model.PaymentId == null)
                {
                    model.PaymentId = paymentID;
                    return View(model);
                }
            }
            else
            {
                if (model.PricePayment != model.Deposit)
                    ModelState.AddModelError("PricePayment", "So tien nhap khong dung vui long nhap lai");
                if (!ModelState.IsValid)
                    return View(model);
            }

            if (model.Method == 2 && model.PaymentId == null)
            {
                model.PricePayment = model.Deposit;
                model.PaymentId = paymentID;
                ModelState.Clear();
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var payment = new Payment
            {
                PaymentId = paymentID,
                Method = model.Method,
                PaymentStatus = 1,
                Price = model.Deposit
            };


            var booking = new Booking
            {
                BookingId = await getVaildIDByBooking(),
                VenueId = model.VenueId,
                OrganizerId = HttpContext.Session.GetString("UserId"),
                DateStart = model.DateStart,
                DateEnd = model.DateEnd,
                BookingDate = DateTime.Now,
                BookingStatus = -1,
                Deposit = model.Deposit,
                PaymentId = payment.PaymentId
            };
            await _DbContext.Payments.AddAsync(payment);
            await _DbContext.Bookings.AddAsync(booking);
            await _DbContext.SaveChangesAsync();
            return RedirectToAction("ListVenue", "Organizer");
        }

        public async Task<IActionResult> ListEvent(String? searchName)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var OrganizerID = HttpContext.Session.GetString("UserId");
            List<Event> le;
            if (searchName == null)
                le = await _DbContext.Events.Where(e => e.OrganizerId == OrganizerID && e.EventStatus != 3).ToListAsync();
            else
                le = await _DbContext.Events.Where(e => e.Name.StartsWith(searchName) && e.OrganizerId == OrganizerID && e.EventStatus != 3).ToListAsync();

            ViewData["Search"] = searchName;
            return View(le);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEditEvent(String? EventID)
        {
            var OrganizerID = HttpContext.Session.GetString("UserId");
            
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var Venues = await _DbContext.Bookings.Where(p => p.OrganizerId == OrganizerID && p.BookingStatus == 1).Select(p=> p.Venue).Distinct().ToListAsync();    
            // Create
            if (EventID == null)
            {
                return View(new EventBookingViewModel
                {
                    EventId = await getVaildIDByEvent(),
                    Name = "",
                    Venues = Venues,
                    Description = null,
                    StartTime = null,
                    EndTime = null,
                    IsPublic = true,
                    EventStatus = null,
                    VenueId = null,
                    Image = null
                });
            }
            // Edit
            else
            {
                var e = await _DbContext.Events.FindAsync(EventID);
                return View(new EventBookingViewModel
                {
                    EventId = e.EventId,
                    Name = e.Name,
                    Venues = Venues,
                    Description = e.Description,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    IsPublic = e.IsPublic,
                    EventStatus = e.EventStatus,
                    VenueId = e.VenueId,
                    Image = e.Image
                });
            }
                
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditEvent(EventBookingViewModel model, IFormFile ImageFile)
        {
            var OrganizerID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            if (model.StartTime == null)
                ModelState.AddModelError("StartTime", "Yeu cau nhap Date Start");
            if (model.EndTime == null)
                ModelState.AddModelError("EndTime", "Yeu cau nhap Date End");
            if (model.EndTime < model.StartTime && model.StartTime > DateTime.Now)
                ModelState.AddModelError("EndTime", "Loi thoi gian khong hop le");
            var bookings = await _DbContext.Bookings.Where(p => p.VenueId == model.VenueId && p.OrganizerId == OrganizerID).ToListAsync();
            bool check = false;
            foreach(var booking in bookings)
            {
                if ((model.StartTime > booking.DateStart && model.EndTime < booking.DateEnd))
                {
                    check = true;
                    break;
                }
            }    
            if(!check)
                ModelState.AddModelError("EndTime", "Loi thoi gian nam ngoai khoang thoi gian booking");
            if (model.VenueId == null)
                ModelState.AddModelError("VenueId", "Không chọn địa điểm và không có địa điểm để chọn");
            if(ImageFile == null || ImageFile.Length == 0)
                ModelState.AddModelError("Image", "Vui lòng chọn ảnh cho sự kiện.");

            if (!ModelState.IsValid)
            {
                var Venues = await _DbContext.Bookings.Where(p => p.OrganizerId == OrganizerID && p.BookingStatus == 1).Select(p => p.Venue).ToListAsync();
                model.Venues = Venues;
                return View(model);
            }
            var ms = new MemoryStream();
            await ImageFile.CopyToAsync(ms);
            // Create
            if (model.EventStatus == null)
            {
                var e = new Event
                {
                    EventId = model.EventId,
                    OrganizerId = OrganizerID,
                    VenueId = model.VenueId,
                    Name = model.Name,
                    Description = model.Description,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    IsPublic = model.IsPublic,
                    EventStatus = -1,
                    Image = ms.ToArray()
                };
                await _DbContext.Events.AddAsync(e);
                await _DbContext.SaveChangesAsync();
            }
            // Edit
            else
            {
                var original = await _DbContext.Events.SingleOrDefaultAsync(p => p.EventId == model.EventId);

                original.Name = model.Name;
                original.Description = model.Description;
                original.StartTime = model.StartTime;
                original.EndTime = model.EndTime;
                original.IsPublic = model.IsPublic;
                original.EventStatus = model.EventStatus;
                original.Image = ms.ToArray();

                await _DbContext.SaveChangesAsync();
            }
            return RedirectToAction("ListEvent", "Organizer");
        }


        [HttpGet]
        public async Task<IActionResult> ViewDetailMyEvent(String EventID)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var Event = await _DbContext.Events.SingleOrDefaultAsync(p => p.EventId == EventID);
            var Venue = await _DbContext.Venues.SingleOrDefaultAsync(p => p.VenueId == Event.VenueId);
            var Booking = await _DbContext.Bookings.SingleOrDefaultAsync(p => p.VenueId == Venue.VenueId && p.OrganizerId == Event.OrganizerId && (Event.StartTime > p.DateStart && Event.EndTime < p.DateEnd));
            var Payment = await _DbContext.Payments.SingleOrDefaultAsync(p => p.PaymentId == Booking.PaymentId);

            return View(new EventTicketVenueViewModel
            {
                EventId = Event.EventId,
                Name = Event.Name,
                Description = Event.Description,
                StartTime = Event.StartTime,
                EndTime = Event.EndTime,
                IsPublic = Event.IsPublic,
                EventStatus = Event.EventStatus,
                NamePlace = Venue.Name,
                PricePlace = Payment.Price,
                Image = Event.Image
            });
        }

        [HttpPost]
        public async Task<IActionResult> CancelEvent(String EventID)
        {
            var Event = await _DbContext.Events.SingleOrDefaultAsync(e => e.EventId == EventID);

            Event.EventStatus = 3;
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("ListEvent", "Organizer");
        }

        [HttpGet]
        public async Task<IActionResult> ListTicket(String? searchName)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var OrganizerID = HttpContext.Session.GetString("UserId");

            List<ListTicketViewModel> lt = new List<ListTicketViewModel>();
            var tickets = await _DbContext.Tickets.Where(t => _DbContext.Events.Any(e => e.EventId == t.EventId && e.OrganizerId == OrganizerID)).ToListAsync();

            if (searchName != null)
            {
                foreach (var ticket in tickets)
                {
                    var Event = await _DbContext.Events.SingleOrDefaultAsync(p => p.EventId == ticket.EventId && p.Name.StartsWith(searchName));
                    if (Event == null)
                        continue;
                    lt.Add(new ListTicketViewModel
                    {
                        TicketId = ticket.TicketId,
                        TicketType = ticket.TicketType,
                        Price = ticket.Price,
                        TotalQuantity = ticket.TotalQuantity,
                        SoldQuantity = ticket.SoldQuantity,
                        Name = Event.Name
                    });
                }
            }
            else
            {
                foreach (var ticket in tickets)
                {
                    var Event = await _DbContext.Events.SingleOrDefaultAsync(p => p.EventId == ticket.EventId);
                    lt.Add(new ListTicketViewModel
                    {
                        TicketId = ticket.TicketId,
                        TicketType = ticket.TicketType,
                        Price = ticket.Price,
                        TotalQuantity = ticket.TotalQuantity,
                        SoldQuantity = ticket.SoldQuantity,
                        Name = Event.Name
                    });
                }
            }    
            return View(lt);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEditTicket(String? TicketID)
        {
            var OrganizerID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var le = await _DbContext.Events.Where(p => p.OrganizerId == OrganizerID && p.EventStatus == 1).ToListAsync();
            // Create
            if (TicketID == null)
            {
                return View(new TicketEventViewModel
                {
                    TicketId = await getVaildIDByTicket(),
                    EventId = null,
                    CommissionId = null,
                    TicketType = 1,
                    Price = null,
                    TotalQuantity = null,
                    Events = le
                });
            }
            // Edit
            else
            {
                var t = await _DbContext.Tickets.SingleOrDefaultAsync(p => p.TicketId == TicketID);
                return View(new TicketEventViewModel
                {
                    TicketId = t.TicketId,
                    EventId = t.EventId,
                    CommissionId = t.CommissionId,
                    TicketType = t.TicketType,
                    Price = t.Price,
                    TotalQuantity = t.TotalQuantity,
                    Events = le
                });
            }    
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditTicket(TicketEventViewModel model)
        {
            var OrganizerID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

           
            if(model.EventId == null)
                ModelState.AddModelError("EventId", "Không chọn hoặc không có sự kiện. Nếu không có sự kiện vui lòng tạo thêm sự kiện và chờ duyệt");
            if (model.TotalQuantity <= 0)
                ModelState.AddModelError("TotalQuantity", "Total Quantity lon hon 0");
            if (model.Price <= 0)
                ModelState.AddModelError("Price", "Price lon hon 0");
            
            if (!ModelState.IsValid)
            {
                var le = await _DbContext.Events.Where(p => p.OrganizerId == OrganizerID).ToListAsync();
                model.Events = le;
                return View(model);
            }    

            var commission = await _DbContext.Commissions.OrderBy(id => id).LastOrDefaultAsync(p => p.CommissionType == 1);
            //Create
            if (model.CommissionId == null)
            {
                var t = await _DbContext.Tickets.SingleOrDefaultAsync(t => t.EventId == model.EventId);
                if (t != null && t.TicketType == model.TicketType)
                    ModelState.AddModelError("TicketType", "Loại vé này của sự kiện này đã được tạo rồi");
                if (!ModelState.IsValid)
                {
                    var le = await _DbContext.Events.Where(p => p.OrganizerId == OrganizerID).ToListAsync();
                    model.Events = le;
                    return View(model);
                }    
                var ticket = new Ticket
                {
                    TicketId = model.TicketId,
                    EventId = model.EventId,
                    CommissionId = commission.CommissionId,
                    TicketType = model.TicketType,
                    Price = model.Price,
                    TotalQuantity = model.TotalQuantity,
                    SoldQuantity = 0,
                };
                await _DbContext.Tickets.AddAsync(ticket);
                await _DbContext.SaveChangesAsync();
            }
            // Edit
            else
            {
                var ticket = await _DbContext.Tickets.SingleOrDefaultAsync(p => p.TicketId == model.TicketId);

                ticket.TicketType = model.TicketType;
                ticket.Price = model.Price;
                ticket.TotalQuantity = model.TotalQuantity;

                await _DbContext.SaveChangesAsync();
            }

            return RedirectToAction("ListTicket", "Organizer");
        }

        public async Task<IActionResult> DeleteTicket(String TicketID)
        {
            var ticket = await _DbContext.Tickets.SingleOrDefaultAsync(p => p.TicketId == TicketID);
            if(ticket.SoldQuantity == 0)
            {
                _DbContext.Tickets.Remove(ticket);
                await _DbContext.SaveChangesAsync();
            }

            return RedirectToAction("ListTicket", "Organizer");
        }

        public async Task<IActionResult> RevenueOrganizer(String type, DateTime? date, int? month)
        {
            var OrganizerID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            ViewData["Type"] = type;

            var events = await _DbContext.Events.Where(e => e.OrganizerId == OrganizerID).ToListAsync();

            List<Transaction> lt = new List<Transaction>();
            foreach(var e in events)
            {
                var tickets = await _DbContext.Tickets.Where(t => t.EventId == e.EventId).Include(t => t.Commission).Include(t => t.Event).ToListAsync();
                foreach(var t in tickets)
                {
                    var transactions = await _DbContext.Transactions.Where(tr => tr.TicketId == t.TicketId).Include(tr => tr.Ticket).Include(tr => tr.Payment).ToListAsync();
                    lt.AddRange(transactions);
                }
            }    
            if(type == "date" && date != null)
            {
                lt = lt.Where(t => t.TransactionDate != null && t.TransactionDate.Value.Date == date.Value.Date && t.TransactionDate.Value.Month == date.Value.Month && t.TransactionDate.Value.Year == date.Value.Year).ToList();
            }    
            if(type == "month" && date != null)
            {
                lt = lt.Where(t => t.TransactionDate != null && t.TransactionDate.Value.Month == month).ToList();
            }

            List<RevenueReportEventViewModel> lvr = new List<RevenueReportEventViewModel>();

            if(type == "event")
            {
                lvr = lt.GroupBy(t => new { t.Ticket.Event.EventId, t.Ticket.Event.Name, Commission = t.Ticket.Commission.Percentage ?? 0 })
                    .Select(g => new RevenueReportEventViewModel
                    {
                        EventName = g.Key.Name,
                        SoldQuantity = g.Sum(t => t.Quantity ?? 0),
                        TotalRevenue = g.Sum(t => t.Payment.Price ?? 0),
                        NetRevenue = g.Sum(t => (t.Payment.Price ?? 0) * (1 - (decimal)g.Key.Commission / 100))
                    }).ToList();
            }
            ViewData["TotalOverallRevenue"] = lt.Sum(t => t.Payment.Price ?? 0);
            ViewData["TotalNetOverallRevenue"] = lt.Sum(t =>
            {
                var commission = t.Ticket.Commission.Percentage ?? 0;
                return Math.Round((t.Payment.Price ?? 0) * (1 - (decimal)commission / 100), 2);

            });

            return View(lvr);
        }

        private async Task<Boolean> checkBookingConflictByVenue(DateTime startTime, DateTime endTime, String VenueID)
        {
            var lb = await _DbContext.Bookings.Where(b => b.VenueId == VenueID).ToListAsync();

            foreach(var b in lb)
            {
                if (!(startTime > b.DateEnd || endTime < b.DateStart))
                    return false;
            }
            return true;
        }

        private async Task<String> getVaildIDByBooking()
        {
            var lID = await _DbContext.Bookings.Select(p => p.BookingId).OrderBy(id => id).ToListAsync();
            int x = 1;
            foreach (var a in lID)
            {
                if (Convert.ToInt32(a) != x)
                    break;
                x++;
            }

            return x.ToString("D8");
        }

        private async Task<String> getVaildIDByPayment()
        {
            var lID = await _DbContext.Payments.Select(p => p.PaymentId).OrderBy(id => id).ToListAsync();
            int x = 1;
            foreach (var a in lID)
            {
                if (Convert.ToInt32(a) != x)
                    break;
                x++;
            }

            return x.ToString("D8");
        }

        private async Task<String> getVaildIDByEvent()
        {
            var lID = await _DbContext.Events.Select(p => p.EventId).OrderBy(id => id).ToListAsync();
            int x = 1;
            foreach (var a in lID)
            {
                if (Convert.ToInt32(a) != x)
                    break;
                x++;
            }

            return x.ToString("D8");
        }

        private async Task<String> getVaildIDByTicket()
        {
            var lID = await _DbContext.Tickets.Select(p => p.TicketId).OrderBy(id => id).ToListAsync();
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
