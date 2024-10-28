using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.Mappers
{
    public class UserConverter
    {
        public DataResponseUser EntityToDTO(User user)
        {
            return new DataResponseUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Avatar = user.Avatar,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                Gender = user.Gender,
                RoleId = user.RoleId,
                RoleName = user.Role != null ? user.Role.RoleName : "Unknown",
                CreateTime = user.CreateTime,
                IsActive = user.IsActive,
                StatusId = user.StatusId,
                UpdateTime = user.UpdateTime,
            };
        }
    }
}
