using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.Response_Models.DataUsers
{
    public class DataResponseUser : DataResponseBase
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool Gender {  get; set; }
        public string Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int StatusId { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
