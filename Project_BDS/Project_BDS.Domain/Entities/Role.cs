using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
