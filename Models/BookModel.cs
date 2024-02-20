namespace WebLibrary.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string ImageUrl {  get; set; }
        public DateTime Date { get; set; }
        public int? Amount { get; set; }
        public ICollection<TagModel>? Tags { get; set; }
        public ICollection<OrderHistoryModel>? OrderHistory { get; set; }
    }
}
