using Microsoft.AspNetCore.Mvc;
using HoldEvent.Models.Entities;
using HoldEvent.Models;
using BCrypt.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using System;
namespace HoldEvent.Controllers
{
    public class AccountController : Controller
    {
        
        private readonly Pbl3Context _DbContext;

        public AccountController(Pbl3Context DbContext)
        {
            _DbContext = DbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = await _DbContext.Accounts.FirstOrDefaultAsync(a => a.UserName == model.UserName);

                if (account == null || !VerifyPassword(model.Password, account.PasswordHash))
                {
                    ModelState.AddModelError("", "Invaild Login");
                    return View(model);
                }
                var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.UserId == account.UserId);

                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("UserId", user.UserId);
                if (user.Role == "Admin")
                {
                    HttpContext.Session.SetString("Role", "Admin");
                    return RedirectToAction("IndexAdmin", "Admin");
                }
                if(user.Role == "User")
                    HttpContext.Session.SetString("Role", "User");
                if(user.Role == "Organizer")
                    HttpContext.Session.SetString("Role", "Organizer");
                if(user.Role == "OwnPlace")
                    HttpContext.Session.SetString("Role", "OwnPlace");
                ViewBag.CurrentUser = user;
                return RedirectToAction("Index", "Home"); 
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult RegisterStep1()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStep1(RegisterViewModel model)
        {
            ModelState.Remove(nameof(model.FullName));
            ModelState.Remove(nameof(model.Email));
            ModelState.Remove(nameof(model.PhoneNumber));
            ModelState.Remove(nameof(model.Address));
            ModelState.Remove(nameof(model.DayOfBirth));
            ModelState.Remove(nameof(model.Gender));

            if (!ModelState.IsValid)
                return View(model);

            var errorsAccount =await checkVaildAccount(model.UserName, model.Password);
            foreach(var e in errorsAccount)
            {
                ModelState.AddModelError(e.Key, e.Value);
            }    
            if(ModelState.IsValid)
            {
                return View("RegisterStep2", model);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStep2(RegisterViewModel model)
        {
            ModelState.Remove(nameof(model.UserName));
            ModelState.Remove(nameof(model.Password));
            ModelState.Remove(nameof(model.ConfirmPassword));
            if (!ModelState.IsValid)
                return View(model);

            if (!UserController.checkEmail(model.Email))
                ModelState.AddModelError("Email", "Email khong hop le");

            if (!UserController.checkNumberPhone(model.PhoneNumber))
                ModelState.AddModelError("PhoneNumber", "Phone Number khong hop le");
            if (!UserController.checkDayOfBirth(model.DayOfBirth))
                ModelState.AddModelError("DayOfBirth", "Day Of Birth khong hop le");

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserId = await getVaildIDByUser(),
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    DayOfBirth = model.DayOfBirth,
                    Gender = model.Gender,
                };

                var account = new Account
                {
                    AccountId = await getVaildIDByAccount(),
                    UserId = user.UserId,
                    UserName = model.UserName,
                    PasswordHash = HashPassword(model.Password),
                    CreateAtDay = DateTime.Now,
                };
                await _DbContext.Users.AddAsync(user);
                await _DbContext.Accounts.AddAsync(account);
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(model);
        }   
        
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(RegisterViewModel model)
        {
            var account = await _DbContext.Accounts.SingleOrDefaultAsync(p => p.UserName == model.UserName);
            var user = await _DbContext.Users.SingleOrDefaultAsync(p => p.Email == model.Email && p.PhoneNumber == model.PhoneNumber);
            if(account == null)
            {
                ModelState.AddModelError("UserName", "Ten dang nhap khong ton tai");
                return View();
            }

            if(user == null)
            {
                ModelState.AddModelError("", "Khong ton tai user co email va Phone Number nhu ban ghi");
                return View();
            }    
            if(account.UserId != user.UserId)
            {
                ModelState.AddModelError("", "ten dang nhap khong khop voi user co email va Phone Number ban ghi");
                return View();
            }

            if(model.FullName == null)
            {
                model.FullName = user.FullName;
                ModelState.Clear();
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);


            account.PasswordHash = HashPassword(model.Password);
            await _DbContext.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(String UserID)
        {
            var Role = HttpContext.Session.GetString("Role");
            ViewData["Role"] = Role;
            var account = await _DbContext.Accounts.SingleOrDefaultAsync(p => p.UserId == UserID);
            if (account == null)
            {
                return RedirectToAction("Index", "Home"); ;
            }
            return View(new ChangePasswordViewModel
            {
                AccountID = account.AccountId,
                OldPasswordHash = account.PasswordHash,
                newPassword = null,
                ConfirmNewPassword = null

            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);
            var account = await _DbContext.Accounts.SingleOrDefaultAsync(p=> p.AccountId == model.AccountID);
            if(account != null)
            {
                if (!VerifyPassword(model.OldPasswordHash, account.PasswordHash))
                {
                    ModelState.AddModelError("OldPasswordHash", "mat khau khong dung");
                    return View(model);
                }
                if(model.newPassword != model.ConfirmNewPassword)
                {
                    ModelState.AddModelError("ConfirmNewPassword", "xac nhan lai mat khau khong dung");
                    return View(model);
                }
                account.PasswordHash = HashPassword(model.newPassword);
                await _DbContext.SaveChangesAsync();
            }  
            return RedirectToAction("Index", "Home");
        }

        private String HashPassword(String password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private Boolean VerifyPassword(String password, String hashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }

        private async Task< String> getVaildIDByAccount()
        {
            var lID = await _DbContext.Accounts.Select(p => p.AccountId).OrderBy(id => id).ToListAsync();
            int x = 1;
            foreach (var a in lID)
            {
                if (Convert.ToInt32(a) != x)
                    break;
                x++;
            }

            return x.ToString("D8");
        }

        private async Task<String> getVaildIDByUser()
        {
            var lID = await _DbContext.Users.Select(p => p.UserId).OrderBy(id => id).ToListAsync();
            int x = 1;
            foreach (var a in lID)
            {
                if (Convert.ToInt32(a) != x)
                    break;
                x++;
            }

            return x.ToString("D8");
        }

        private async Task< Boolean> checkExistUserName(String userName)
        {
            var a = await _DbContext.Accounts.SingleOrDefaultAsync(p => p.UserName == userName);
            if (a == null)
                return false;
            return true;
        }

        private async Task<Dictionary<String, String>> checkVaildAccount(String userName, String password)
        {
            var errors = new Dictionary<String, String>();
            // check tài khoản
            if (userName.Length < 6)
                errors["UserName"] = "Acount length >= 6";
            else
                if (userName.Contains(" "))
                    errors["UserName"] = "User Name khong duoc co khoang trang";
                else
                    if (await checkExistUserName(userName))
                        errors["UserName"] = "User Name da ton tai";
                    else
                        if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9_.]+$"))
                            errors["UserName"] = "User Name chi chua ki tu thuong, ki tu hoa, so";
                        else
                            if (!Regex.IsMatch(userName, @"^[a-zA-Z]"))
                                errors["UserName"] = "User Name phai bat dau bang chu cai";

            // check mat khau
            if (password.Length < 8)
                errors["Password"] = "Chieu dai cua Password lon hon 8";
            else
                if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).+$"))
                    errors["Password"] = "Mat khau phai chua 1 chu thuong, 1 chu in hoa, 1 chu so, 1 ki tu dac biet";

            return errors;
        }
    }
}
