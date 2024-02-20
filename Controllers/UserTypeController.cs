using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class UserTypeController : Controller
    {
        private readonly WLDbContext _db;

        public UserTypeController(WLDbContext db) {
            _db = db;
        }

        public IActionResult Index()
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);
            if (!currentUser.UserType.IsAdmin)
            {
                return Redirect("/Home");
            }

            List<UserTypeModel> userTypes = new List<UserTypeModel>();
            userTypes = _db.UserTypes.ToList();

            var users = _db.Users.ToList();
            var roles = _db.UserTypes.ToList();
            ViewBag.Users = users;
            ViewBag.UserRoles = roles;


            return View(userTypes);
        }
        [HttpPost]
        public ActionResult UpdateUserTypes(List<UserTypeModel> userTypes)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);
            if (!currentUser.UserType.IsAdmin)
            {
                return Redirect("/Home");
            }

            foreach (var userType in userTypes)
            {
                var existingUserType = _db.UserTypes.Find(userType.Id);

                existingUserType.Name = userType.Name;
                existingUserType.IsAdmin = userType.IsAdmin;
                existingUserType.CanAcceptRegistration = userType.CanAcceptRegistration;
                existingUserType.CanManageBooks = userType.CanManageBooks;
                existingUserType.CanManageReaders = userType.CanManageReaders;
                existingUserType.CanManageRentals = userType.CanManageRentals;
                existingUserType.CanManageLimits = userType.CanManageLimits;
                existingUserType.CanMakePosts = userType.CanMakePosts;

                _db.UserTypes.Update(existingUserType);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult AddUserType(string typeName)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.IsAdmin)
            {
                return Redirect("/Home");
            }

            UserTypeModel newUserType = new UserTypeModel
            {
                Name = typeName,
                IsAdmin = false,
                CanAcceptRegistration = false,
                CanManageBooks = false,
                CanManageReaders = false,
                CanManageRentals = false,
                CanManageLimits = false,
                CanMakePosts = false
            };

            _db.UserTypes.Add(newUserType);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateUserRole(int userId, int userRoles)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);
            if (!currentUser.UserType.IsAdmin)
            {
                return Redirect("/Home");
            }

            var user = _db.Users.Find(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.UserType = _db.UserTypes.Find(userRoles);

            _db.Users.Update(user);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}