namespace WebLibrary.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required UserTypeModel UserType { get; set; }
        public ICollection<OrderHistoryModel>? OrderHistory { get; set; }
    }
}