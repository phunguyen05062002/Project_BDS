using Project_BDS.Application.Payloads.Response_Models.DataProduct;
using Project_BDS.Application.Payloads.Response_Models.DataUsers;
using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.Mappers
{
    public class RoleConverter
    {
        public static DataResponeseRole EntityToDTO(Role role)
        {
            return new DataResponeseRole
            {
                Id = role.Id,
                RoleCode = role.RoleCode,
                RoleName = role.RoleName,
            };
        }
    }
}
