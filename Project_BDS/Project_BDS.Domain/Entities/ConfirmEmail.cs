using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.Entities
{
    public class ConfirmEmail : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime RequiredDateTime { get; set; }
        public DateTime ExpiredDateTime { get; set; }
        public string Code { get; set; }
        public bool IsConfirm { get; set; }
    }
}
