using Microsoft.AspNetCore.Mvc;
using HoldEvent.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using HoldEvent.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace HoldEvent.Controllers
{
    public class UserController : Controller
    {
        private readonly Pbl3Context _DbContext;

        public UserController(Pbl3Context DbContext)
        {
            _DbContext = DbContext;
        }

        public static Boolean checkEmail(String Email)
        {
            var index1 = Email.IndexOf('@');
            if (index1 <= 0 || index1 != Email.LastIndexOf('@'))
                return false;

            var index2 = Email.LastIndexOf('.');
            if (index2 < index1)
                return false;
            if (Email.EndsWith('.'))
                return false;
            return true;
        }

        public static Boolean checkNumberPhone(String NumberPhone)
        {
            if(!NumberPhone.StartsWith("0"))
                return false;
            if (NumberPhone.Length != 10)
                return false;
            foreach (char c in NumberPhone)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;

        }

        public static Boolean checkDayOfBirth(DateTime? DayOfBirth)
        {
            return DayOfBirth < DateTime.Now;
        }

        // xem thong tin
        [HttpGet]
        public async Task< IActionResult> UserInformation()
        {
            var UserID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var user = await _DbContext.Users.FindAsync(UserID);
            return View(user);
        }

        [HttpPost]
        public IActionResult UserInformationPost()
        {
            return RedirectToAction("Index", "Home");
        }

        // chinh sua thong tin
        [HttpGet]
        public async Task<IActionResult> UserInformationEdit()
        {
            var UserID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var user = await _DbContext.Users.FindAsync(UserID);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UserInformationEdit(User userModel)
        {
            var UserID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            if (String.IsNullOrWhiteSpace(userModel.FullName))
                ModelState.AddModelError("FullName", "FullName is required");
            if (String.IsNullOrWhiteSpace(userModel.Email))
                ModelState.AddModelError("Email", "Email is required");
            if (String.IsNullOrWhiteSpace(userModel.PhoneNumber))
                ModelState.AddModelError("PhoneNumber", "PhoneNumber is required");

            if (!ModelState.IsValid)
            {
                return View(userModel);
            }
            if (!UserController.checkEmail(userModel.Email))
                ModelState.AddModelError("Email", "Email khong hop le");

            if (!UserController.checkNumberPhone(userModel.PhoneNumber))
                ModelState.AddModelError("PhoneNumber", "Phone Number khong hop le");
            if (!UserController.checkDayOfBirth(userModel.DayOfBirth))
                ModelState.AddModelError("DayOfBirth", "Day Of Birth khong hop le");
            if (ModelState.IsValid)
            {

                var user = await _DbContext.Users.SingleOrDefaultAsync(p=> p.UserId == userModel.UserId);
                if (user != null)
                {
                    user.FullName = userModel.FullName;
                    user.Email = userModel.Email;
                    user.PhoneNumber = userModel.PhoneNumber;
                    user.Address = userModel.Address;
                    user.DayOfBirth = userModel.DayOfBirth;
                    user.Gender = userModel.Gender;

                    await _DbContext.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Home");
            }
            return View(userModel);
            
        }

        public IActionResult ChangePassword()
        {
            var UserID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            return RedirectToAction("ChangePassword", "Account", new { UserID = UserID });
        }
        [HttpGet]
        public async Task<IActionResult> Feedback()
        {
            var UserID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            return View(new Feedback
            {
                FeedbackId = await getVaildIDByFeedback(),
                UserId = UserID,
                Title = null,
                Content = null,
                CreateAtDay = null,
                FeedbackState = 0,
            });
        }
        [HttpPost]
        public async Task<IActionResult> Feedback(Feedback Model)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            if (String.IsNullOrWhiteSpace(Model.Title))
                ModelState.AddModelError("Title", "Title khong duoc de trong");
            if (String.IsNullOrWhiteSpace(Model.Content))
                ModelState.AddModelError("Content", "Content khong duoc de trong");
            if (!ModelState.IsValid)
                return View(Model);
            var feedback = new Feedback
            {
                FeedbackId = Model.FeedbackId,
                UserId = Model.UserId,
                Title = Model.Title,
                Content = Model.Content,
                CreateAtDay = DateTime.Now,
                FeedbackState = Model.FeedbackState
            };
            await _DbContext.Feedbacks.AddAsync(feedback);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> ListFeedback()
        {
            var UserID = HttpContext.Session.GetString("UserId");
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            var lf = await _DbContext.Feedbacks.Where(p => p.UserId == UserID).ToListAsync();
            return View(lf);
        }


        [HttpGet]
        public async Task<IActionResult> ViewFeedbackRespond(String FeedbackID)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var feedback = await _DbContext.Feedbacks.SingleOrDefaultAsync(p => p.FeedbackId == FeedbackID);
            var respone =await _DbContext.Supports.SingleOrDefaultAsync(p => p.FeedbackId == FeedbackID);
            if (respone == null)
                return View(new FeedbackSupportViewModel
                {
                    SupportId = null,
                    FeedbackId = feedback.FeedbackId,
                    UserId = feedback.UserId,
                    AdminId = null,
                    Title = feedback.Title,
                    ContentFeedback = feedback.Content,
                    CreateAtDayFeedback = feedback.CreateAtDay,
                    ContentSupport = null,
                    CreateAtDaySupport = null,
                });
            return View(new FeedbackSupportViewModel
            {
                SupportId = respone.SupportId,
                FeedbackId = feedback.FeedbackId,
                UserId = feedback.UserId,
                AdminId = respone.AdminId,
                Title = feedback.Title,
                ContentFeedback = feedback.Content,
                CreateAtDayFeedback = feedback.CreateAtDay,
                ContentSupport = respone.Content,
                CreateAtDaySupport = respone.CreateAtDay
            });

        }

        [HttpGet]
        public async Task<IActionResult> ViewDetailEvent(String EventID)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            var e = await _DbContext.Events.SingleOrDefaultAsync(p => p.EventId == EventID);
            var tickets = await _DbContext.Tickets.Where(p => p.EventId == e.EventId).ToListAsync();
            var o = await _DbContext.Users.SingleOrDefaultAsync(p => p.UserId == e.OrganizerId);
            var v = await _DbContext.Venues.SingleOrDefaultAsync(p => p.VenueId == e.VenueId);

            if (tickets != null && e.IsPublic == true)
                return View(new EventTicketOrganizerVenueViewModel
                {
                    EventId = e.EventId,
                    NameOrganizer = o.FullName,
                    NameVenue = v.Name,
                    NameEvent = e.Name,
                    Description = e.Description,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    DescriptionVenue = v.Description,
                    Address = v.Address,
                    Email = o.Email,
                    PhoneNumber = o.PhoneNumber,
                    tickets = tickets
                });
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> BookTicket(String EventID)
        {
            var Role = HttpContext.Session.GetString("Role");
            if (Role == null)
                return RedirectToAction("Login", "Account");
            ViewData["Role"] = Role;

            var e = await _DbContext.Events.SingleOrDefaultAsync(p => p.EventId == EventID);

            return View(new TicketTransactionViewModel
            {
                TicketId = null,
                EventId = e.EventId,
                NameEvent = e.Name,
                tickets = await _DbContext.Tickets.Where(p => p.EventId == e.EventId).ToListAsync(),
                Quantity = 1,
            });
        }
        [HttpPost]
        public async Task<IActionResult> BookTicket(TicketTransactionViewModel model)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            if (model.Quantity <= 0)
                ModelState.AddModelError("Quantity", "Quantity phải lớn hơn 0");

            var e = await _DbContext.Events.SingleOrDefaultAsync(p => p.EventId == model.EventId);
            var tickets = await _DbContext.Tickets.Where(p => p.EventId == e.EventId).ToListAsync();
            model.tickets = tickets;
            var ticket = await _DbContext.Tickets.SingleOrDefaultAsync(p => p.TicketId == model.TicketId);
            if(ticket.SoldQuantity == ticket.TotalQuantity)
                ModelState.AddModelError("Quantity", "Loại vé này đã hết");
            if (!ModelState.IsValid)
            {
                return View(model);
            }    

            decimal Price =(decimal) ticket.Price;
            decimal TotalPrice = (decimal)model.Quantity * Price;
            ModelState.Remove("TotalPrice");
            model.TotalPrice = Math.Round(TotalPrice, 2);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentTicket(String TicketID, int Quantity, decimal TotalPrice)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            var ticket = await _DbContext.Tickets.SingleOrDefaultAsync(p => p.TicketId == TicketID);
            return View(new TicketTransactionViewModel
            {
                TicketId = ticket.TicketId,
                TicketType = ticket.TicketType,
                TotalPrice = TotalPrice,
                Quantity = Quantity,
                PaymentId = null,
            });
        }

        [HttpPost]

        public async Task<IActionResult> PaymentTicket(TicketTransactionViewModel model)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var UserID = HttpContext.Session.GetString("UserId");

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
                if (model.PricePayment != model.TotalPrice)
                    ModelState.AddModelError("PricePayment", "So tien nhap khong dung vui long nhap lai");
                if (!ModelState.IsValid)
                    return View(model);
            }

            var payment = new Payment
            {
                PaymentId = paymentID,
                Method = model.Method,
                PaymentStatus = 1,
                Price = model.TotalPrice
            };

            var transaction = new Transaction
            {
                TransactionId = await getVaildIDByTransaction(),
                TicketId = model.TicketId,
                PaymentId = paymentID,
                UserId = UserID,
                Quantity = model.Quantity,
                TransactionDate = DateTime.Now
            };

            var tou = new TicketOfUser
            {
                UserId = UserID,
                TicketId = model.TicketId,
                TransactionId = transaction.TransactionId,
                Status = 1,
            };

            var ticket = await _DbContext.Tickets.SingleOrDefaultAsync(p => p.TicketId == model.TicketId);
            ticket.SoldQuantity += model.Quantity;


            await _DbContext.Payments.AddAsync(payment);
            await _DbContext.Transactions.AddAsync(transaction);
            await _DbContext.TicketOfUsers.AddAsync(tou);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]

        public async Task<IActionResult> MyTicket(String? searchName)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;

            List<TicketUserViewModel> ltu = new List<TicketUserViewModel>();
            if (searchName == null)
            {
                var UserID = HttpContext.Session.GetString("UserId");
                var lt = await _DbContext.Transactions.Where(p => p.UserId == UserID).ToListAsync();
                foreach(var t in lt)
                {
                    var payment = await _DbContext.Payments.SingleOrDefaultAsync(p => p.PaymentId == t.PaymentId);
                    var ticket = await _DbContext.Tickets.SingleOrDefaultAsync(p => p.TicketId == t.TicketId);
                    var e = await _DbContext.Events.SingleOrDefaultAsync(p => p.EventId == ticket.EventId);
                    var venue = await _DbContext.Venues.SingleOrDefaultAsync(p => p.VenueId == e.VenueId);
                    var ticketOfUser = await _DbContext.TicketOfUsers.SingleOrDefaultAsync(p => p.TransactionId == t.TransactionId);
                    ltu.Add(new TicketUserViewModel
                    {
                        NameEvent = e.Name,
                        NameVenue = venue.Name,
                        Quantity = t.Quantity,
                        TicketType = ticket.TicketType,
                        Address = venue.Address,
                        Status = ticketOfUser.Status,
                        Price = payment.Price,
                        StartTime = e.StartTime,
                        EndTime = e.EndTime
                    });
                }    
            }
            else
            {
                var UserID = HttpContext.Session.GetString("UserId");
                var lt = await _DbContext.Transactions.Where(p => p.UserId == UserID).ToListAsync();
                foreach (var t in lt)
                {
                    var payment = await _DbContext.Payments.SingleOrDefaultAsync(p => p.PaymentId == t.PaymentId);
                    var ticket = await _DbContext.Tickets.SingleOrDefaultAsync(p => p.TicketId == t.TicketId);
                    var e = await _DbContext.Events.SingleOrDefaultAsync(p => p.EventId == ticket.EventId);
                    if (e.Name.StartsWith(searchName))
                        continue;
                    var venue = await _DbContext.Venues.SingleOrDefaultAsync(p => p.VenueId == e.VenueId);
                    var ticketOfUser = await _DbContext.TicketOfUsers.SingleOrDefaultAsync(p => p.TransactionId == t.TransactionId);
                    ltu.Add(new TicketUserViewModel
                    {
                        NameEvent = e.Name,
                        NameVenue = venue.Name,
                        Quantity = t.Quantity,
                        TicketType = ticket.TicketType,
                        Address = venue.Address,
                        Status = ticketOfUser.Status,
                        Price = payment.Price,
                        StartTime = e.StartTime,
                        EndTime = e.EndTime
                    });
                }
            }
            return View(ltu);
        }

        private async Task<String> getVaildIDByFeedback()
        {
            var lID = await _DbContext.Feedbacks.Select(p => p.FeedbackId).OrderBy(id => id).ToListAsync();
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

        private async Task<String> getVaildIDByTransaction()
        {
            var lID = await _DbContext.Transactions.Select(p => p.TransactionId).OrderBy(id => id).ToListAsync();
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
