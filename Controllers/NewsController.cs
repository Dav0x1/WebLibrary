using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly WLDbContext _db;
        public NewsController(WLDbContext db)
        {
            _db = db;
        }
    
        public IActionResult Index()
        {
            string userEmail = User.Identity.Name;
            UserModel user = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!user.UserType.CanMakePosts)
            {
                return Redirect("/Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string title, string description)
        {
            string userEmail = User.Identity.Name;
            UserModel user = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!user.UserType.CanMakePosts)
            {
                return Redirect("/Home");
            }

            if (title != null && description != null)
            {
                NewsModel news = new NewsModel();
                news.Title = title;
                news.Description = description;
                news.CreatedDate = DateTime.Now;

                _db.News.Add(news);
                _db.SaveChanges();

                ViewBag.Notification = "News created";
            }
            else
            {
                ViewBag.Notification = "Fill the fields";
            }
            return View();
        }
    }
}
