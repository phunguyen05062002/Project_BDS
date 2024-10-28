using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.RequestModels.UserRequests
{
    public class UpdateUserRoleRequest
    {
        public int UserId { get; set; }
        public int NewRoleId { get; set; }
    }
}
