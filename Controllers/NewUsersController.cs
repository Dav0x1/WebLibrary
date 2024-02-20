using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class NewUsersController : Controller
    {
        private readonly WLDbContext _db;

        public NewUsersController(WLDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.CanMakePosts)
            {
                return Redirect("/Home");
            }

            ICollection<UserModel> Users = _db.Users.Include(u => u.UserType).Where(e => e.UserType.Name == "Unverified").ToList();

            ViewBag.Users = Users;

            return View();
        }
        [HttpPost]
        public ActionResult Index(int buttonId)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.CanMakePosts)
            {
                return Redirect("/Home");
            }

            UserModel user = _db.Users.Include(u => u.UserType).Where(e => e.Id == buttonId).FirstOrDefault();
            user.UserType = _db.UserTypes.FirstOrDefault((e) => e.Name == "Verified");
            _db.SaveChanges();

            return Redirect("/NewUsers");
        }
    }
}
