using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class RentalsController : Controller
    {
        readonly private WLDbContext _db;

        public RentalsController(WLDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);
            
            if (!currentUser.UserType.CanManageRentals)
            {
                return Redirect("/Home");
            }

            var Users = _db.Users.ToList();
            ViewBag.Users = Users;

            return View();
        }
        [HttpPost]
        public IActionResult Index(int userId)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.CanManageRentals)
            {
                return Redirect("/Home");
            }

            var userRentals = _db.OrderHistory.Include(u=> u.Books).Where(r => r.User.Id == userId).ToList();

            return View("Rentals",userRentals);
        }

        [HttpPost]
        public IActionResult Return(int rentalId)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.CanManageRentals)
            {
                return Redirect("/Home");
            }

            OrderHistoryModel rental = _db.OrderHistory.Include(e=>e.Books).FirstOrDefault(r => r.Id == rentalId);
            rental.haveReturned = true;
            foreach (var book in rental.Books)
            {
                book.Amount++;
            }

            _db.OrderHistory.Update(rental);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
    
}
