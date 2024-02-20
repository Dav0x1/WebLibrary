using Microsoft.AspNetCore.Mvc;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly WLDbContext _db;
        public RegistrationController(WLDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register([FromForm] string Name, [FromForm] string Surname, [FromForm] string Email, [FromForm] string Password)
        {
            bool isExist = _db.Users.Any(u => u.Email == Email);
            string Notification = "";

            // TODO: Implement data validation

            if (isExist)
                Notification = "User with such e-mail already exists";
            else
            {
                UserModel userModel = new UserModel
                {
                    Name = Name,
                    Surname = Surname,
                    Email = Email,
                    Password = Password,
                    UserType = _db.UserTypes.FirstOrDefault(type => type.Name == "Unverified")
                };

                _db.Users.Add(userModel);
                _db.SaveChanges();
                Notification = "Registration Successful";
            }

            ViewBag.Notification = Notification;
            return View("Index");
        }
    }
}