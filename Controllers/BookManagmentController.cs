using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class BookManagmentController : Controller
    {
        private readonly WLDbContext _db;

        public BookManagmentController(WLDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.CanManageBooks)
            {
                return Redirect("/Home");
            }

            ICollection<BookModel> Books = _db.Books.ToList();

            ViewBag.Books = Books;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(string title, string description,string author, IFormFile image,string Tags)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.CanManageBooks)
            {
                return Redirect("/Home");
            }
            // Save file
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
            FileInfo fileInfo = new FileInfo(image.FileName);
            string fileName = String.Concat(title.Where(c => !Char.IsWhiteSpace(c))) + fileInfo.Extension; 
            string fileNameWithPath = Path.Combine(path, fileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            if (title!=null && description != null && author != null)
            {
                BookModel Book = new BookModel();
                Book.Title = title;
                Book.Description = description;
                Book.Author = author;
                Book.ImageUrl = fileName;
                Book.Date = DateTime.Now;
                Book.Amount = 1;
                if (Tags != null)
                {
                    string[] tagArray = Tags.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var tagName in tagArray)
                    {
                        bool tagExists = _db.Tags.Any(t => t.Name == tagName);

                        if (!tagExists)
                        {
                            TagModel Tag = new TagModel { Name = tagName };
                            if (Book.Tags == null)
                            {
                                Book.Tags = new List<TagModel>();
                            }

                            Book.Tags.Add(Tag);
                            _db.Tags.Add(Tag);
                        }
                        else
                        {
                            TagModel tag = _db.Tags.FirstOrDefault(e => e.Name == tagName);
                            if (Book.Tags == null)
                            {
                                Book.Tags = new List<TagModel>();
                            }

                            Book.Tags.Add(tag);
                        }

                    }
                }
                _db.Books.Add(Book);
                _db.SaveChanges();
            }

            return Redirect("/BookManagment/Index");
        }
        [HttpPost]
        public ActionResult RemoveBook(int id)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.CanManageBooks)
            {
                return Redirect("/Home");
            }

            var bookToRemove = _db.Books.Find(id);

            if (bookToRemove != null)
            {
                _db.Books.Remove(bookToRemove);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.CanManageBooks)
            {
                return Redirect("/Home");
            }

            var bookToEdit = _db.Books.Find(id);

            if (bookToEdit == null)
            {
                return RedirectToAction("Index");
            }

            var bookModel = new BookModel
            {
                Id = bookToEdit.Id,
                Title = bookToEdit.Title,
                Description = bookToEdit.Description,
                ImageUrl = bookToEdit.ImageUrl,
                Author = bookToEdit.Author,
                Amount = bookToEdit.Amount
            };

            return View("Edit", bookModel); 
        }
        [HttpPost]
        public ActionResult UpdateBook(BookModel updatedBook)
        {
            string userEmail = User.Identity.Name;
            UserModel currentUser = _db.Users.Include(u => u.UserType).FirstOrDefault(u => u.Email == userEmail);

            if (!currentUser.UserType.CanManageBooks)
            {
                return Redirect("/Home");
            }

            var existingBook = _db.Books.Find(updatedBook.Id);

            if (existingBook != null)
            {
                existingBook.Title = updatedBook.Title;
                existingBook.Description = updatedBook.Description;
                existingBook.Author = updatedBook.Author;
                if (updatedBook.ImageUrl != null)
                {
                    existingBook.ImageUrl = updatedBook.ImageUrl;
                }
                existingBook.Amount = updatedBook.Amount;

                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View("Index", updatedBook);
        }
    }
}
