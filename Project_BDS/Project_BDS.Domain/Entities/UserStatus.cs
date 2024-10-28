using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.Entities
{
    public class UserStatus : BaseEntity
    {
        public string StatusCode { get; set; }
        public string StatusName { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
