using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool Gender { get; set; }
        public string Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public int StatusId { get; set; }
        public UserStatus? Status { get; set; }
        public bool? IsActive { get; set; } = true;

        public ICollection<ConfirmEmail>? ConfirmEmails { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
    }
}
