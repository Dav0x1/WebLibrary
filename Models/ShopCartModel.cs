namespace WebLibrary.Models
{
    public class ShopCartModel
    {
        public List<BookModel> Books { get; set; }
        public ShopCartModel()
        {
            Books = new List<BookModel>();
        }
    }
}
