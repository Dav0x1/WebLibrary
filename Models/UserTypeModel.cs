using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLibrary.Models
{
    public class UserTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public bool CanAcceptRegistration { get; set; }
        public bool CanManageBooks {  get; set; }
        public bool CanManageReaders {  get; set; }
        public bool CanManageRentals {  get; set; }
        public bool CanManageLimits {  get; set; }
        public bool CanMakePosts { get; set; }
        public ICollection<UserModel> Users { get; set; }
    }
}