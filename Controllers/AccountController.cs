using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly WLDbContext _db;
        public AccountController(WLDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            string userEmail = User.Identity.Name;
            UserModel user = _db.Users.Include(e => e.UserType).FirstOrDefault(u => u.Email == userEmail);

            return View(user);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View("ChangePassword/Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string cpassword,string npassword)
        {
            string userEmail = User.Identity.Name;
            UserModel user = _db.Users.FirstOrDefault(u => u.Email == userEmail);

            if (cpassword == user.Password)
            {
                user.Password = npassword;
                _db.Users.Update(user);
                _db.SaveChanges();
                ViewBag.Notification = "Password changed";
            }
            else
            {
                ViewBag.Notification = "Incorrect password";
            }
            return View("ChangePassword/Index");
        }
    }
}