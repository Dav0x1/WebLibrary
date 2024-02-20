namespace WebLibrary.Models
{
    public class OrderHistoryModel
    {
        public int Id { get; set; }
        public UserModel User { get; set; }
        public ICollection<BookModel> Books { get; set; }
        public bool haveReturned { get; set; }
    }
}