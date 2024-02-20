using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    public class LoginController : Controller
    {
        private readonly WLDbContext _db;
        private readonly EmailSender _emailSender;
        public LoginController(WLDbContext db, EmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult PasswordRemind()
        {
            return View("PasswordRemind/Index");
        }
        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            string Notification = "";
            UserModel user = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                if (user.Password == password)
                {
                    if (user.UserType.Name == "Unverified")
                    {
                        ViewBag.Notification = "Pending verification";
                        return View();
                    }
                    //----------------------------
                    ClaimsPrincipal principal = CreatePrincipal(user);

                    AuthenticationProperties props = new AuthenticationProperties();

                    await HttpContext.SignInAsync(principal,props);

                    return Redirect("/Account");
                }
                else
                {
                    Notification = "Incorrect password";
                }
            }
            else
            {
                Notification = "User doesn't exist";
            }
            if (Notification != "")
                ViewBag.Notification = Notification;
            return View();
        }
        // POST - PASSWORD REMIND PAGE
        [HttpPost]
        public async Task<IActionResult> PasswordRemind(string email)
        {
            string Notification = "";

            UserModel user = _db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                Notification = "User doesn't exist";
            }
            else
            {
                Notification = "The email has been sent";
                _emailSender.remindPassword(user.Email,user.Password);
            }
            

            if (Notification != "")
                ViewBag.Notification = Notification;
            return View("PasswordRemind/Index");
        }
        ClaimsPrincipal CreatePrincipal(UserModel user)
        {
            ClaimsPrincipal result = new ClaimsPrincipal();

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            result.AddIdentity(identity);

            return result;
        }

    }
}