using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebLibrary.Models;

namespace WebLibrary.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly WLDbContext _db;
        private readonly IHttpContextAccessor _httpContext;
        public BooksController(WLDbContext db, IHttpContextAccessor httpContext)
        {
            _db = db;
            _httpContext = httpContext;
        }
        public IActionResult Index()
        {
            ICollection<BookModel> Books = _db.Books.Include(b => b.Tags).ToList();

            int quantity = 0;

            if (_httpContext.HttpContext.Session.IsAvailable && _httpContext.HttpContext.Session.Keys.Contains("ShopCart"))
            {
                ShopCartModel shopCart = JsonConvert.DeserializeObject<ShopCartModel>(_httpContext.HttpContext.Session.GetString("ShopCart"));
                quantity = shopCart.Books.Count();
            }

            ViewBag.Books = Books;
            ViewBag.CartQuantity = quantity;
            return View();
        }
        [HttpPost]
        public IActionResult Index(string searchTerm, string searchBy)
        {
            IQueryable<BookModel> booksQuery = _db.Books.Include(b => b.Tags);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                // Split the searchTerm into individual search terms and operators
                string[] searchTerms = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Variables to track logical operators
                bool useAnd = true;
                bool useOr = false;
                bool useNot = false;

                foreach (var term in searchTerms)
                {
                    // Check for logical operators
                    if (term.ToUpper() == "AND")
                    {
                        useAnd = true;
                        useOr = false;
                        useNot = false;
                        continue;
                    }
                    else if (term.ToUpper() == "OR")
                    {
                        useAnd = false;
                        useOr = true;
                        useNot = false;
                        continue;
                    }
                    else if (term.ToUpper() == "NOT")
                    {
                        useAnd = false;
                        useOr = false;
                        useNot = true;
                        continue;
                    }

                    // Apply conditions based on the logical operators
                    if (searchBy == "Title")
                    {
                        if (useAnd)
                            booksQuery = booksQuery.Where(b => b.Title.Contains(term));
                        else if (useOr)
                            booksQuery = booksQuery.Where(b => b.Title.Contains(term) || searchTerm.Contains(term));
                        else if (useNot)
                            booksQuery = booksQuery.Where(b => !b.Title.Contains(term));
                    }
                    else if (searchBy == "Tags")
                    {
                        if (useAnd)
                            booksQuery = booksQuery.Where(b => b.Tags.Any(t => t.Name.Contains(term)));
                        else if (useOr)
                            booksQuery = booksQuery.Where(b => b.Tags.Any(t => t.Name.Contains(term) || searchTerm.Contains(term)));
                        else if (useNot)
                            booksQuery = booksQuery.Where(b => !b.Tags.Any(t => t.Name.Contains(term)));
                    }
                    else if (searchBy == "Author")
                    {
                        if (useAnd)
                            booksQuery = booksQuery.Where(b => b.Author.Contains(term));
                        else if (useOr)
                            booksQuery = booksQuery.Where(b => b.Author.Contains(term) || searchTerm.Contains(term));
                        else if (useNot)
                            booksQuery = booksQuery.Where(b => !b.Author.Contains(term));
                    }
                    else if (searchBy == "Description")
                    {
                        if (useAnd)
                            booksQuery = booksQuery.Where(b => b.Description.Contains(term));
                        else if (useOr)
                            booksQuery = booksQuery.Where(b => b.Description.Contains(term) || searchTerm.Contains(term));
                        else if (useNot)
                            booksQuery = booksQuery.Where(b => !b.Description.Contains(term));
                    }
                }
            }

            ICollection<BookModel> books = booksQuery.ToList();

            int quantity = 0;

            if (_httpContext.HttpContext.Session.IsAvailable && _httpContext.HttpContext.Session.Keys.Contains("ShopCart"))
            {
                ShopCartModel shopCart = JsonConvert.DeserializeObject<ShopCartModel>(_httpContext.HttpContext.Session.GetString("ShopCart"));
                quantity = shopCart.Books.Count();
            }

            ViewBag.Books = books;
            ViewBag.CartQuantity = quantity;
            ViewBag.CurrentFilter = searchTerm; // Przekazanie aktualnego filtru do widoku

            return View();
        }


        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            BookModel book = _db.Books.FirstOrDefault(x => x.Id == id);
            if (book.Amount <1)
            {return RedirectToAction("Index");
                
            }
            ShopCartModel shopCart;
            if (_httpContext.HttpContext.Session.IsAvailable && _httpContext.HttpContext.Session.Keys.Contains("ShopCart"))
            {
                shopCart = JsonConvert.DeserializeObject<ShopCartModel>(_httpContext.HttpContext.Session.GetString("ShopCart"));
            }
            else
            {
                shopCart = new ShopCartModel();
            }
            
            shopCart.Books.Add(book);

            _httpContext.HttpContext.Session.SetString("ShopCart", JsonConvert.SerializeObject(shopCart));
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            ShopCartModel shopCart;
            if (_httpContext.HttpContext.Session.IsAvailable && _httpContext.HttpContext.Session.Keys.Contains("ShopCart"))
            {
                shopCart = JsonConvert.DeserializeObject<ShopCartModel>(_httpContext.HttpContext.Session.GetString("ShopCart"));

                var bookToRemove = shopCart.Books.FirstOrDefault(b => b.Id == id);
                if (bookToRemove != null)
                {
                    shopCart.Books.Remove(bookToRemove);
                    _httpContext.HttpContext.Session.SetString("ShopCart", JsonConvert.SerializeObject(shopCart));
                }
            }
            return RedirectToAction("CheckCart");
        }

        [HttpPost]
        public ActionResult MakeOrder()
        {
            ShopCartModel shopCart;
            if (_httpContext.HttpContext.Session.IsAvailable && _httpContext.HttpContext.Session.Keys.Contains("ShopCart"))
            {
                shopCart = JsonConvert.DeserializeObject<ShopCartModel>(_httpContext.HttpContext.Session.GetString("ShopCart"));


                string userEmail = User.Identity.Name; // Pobierz e-mail z ClaimsPrincipal
                UserModel user = _db.Users.FirstOrDefault(u => u.Email == userEmail);

                OrderHistoryModel order = new OrderHistoryModel();

                order.User = user;
                order.Books = new List<BookModel>();
                //order.Books = shopCart.Books;
                foreach (var x in shopCart.Books)
                {
                    order.Books.Add(_db.Books.FirstOrDefault(e=> e.Id == x.Id));
                    _db.Books.FirstOrDefault(e => e.Id == x.Id).Amount--;
                }

                _db.OrderHistory.Add(order);
                _db.SaveChanges();

                _httpContext.HttpContext.Session.Remove("ShopCart");
                ViewBag.Notification = "Order successful! Your book is waiting for you at the library";
            }
            else
            {
                ViewBag.Notification = "Choose books";
            }

            return View("ShopCart");
        }
        [HttpGet]
        public ActionResult CheckCart()
        {
            int quantity = 0;

            if (_httpContext.HttpContext.Session.IsAvailable && _httpContext.HttpContext.Session.Keys.Contains("ShopCart"))
            {
                ShopCartModel shopCart = JsonConvert.DeserializeObject<ShopCartModel>(_httpContext.HttpContext.Session.GetString("ShopCart"));
                quantity = shopCart.Books.Count();

                ICollection<BookModel> Books = shopCart.Books;
                ViewBag.Books = Books;
            }

            ViewBag.CartQuantity = quantity;


            return View("ShopCart");
        }
    }
}