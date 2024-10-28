using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.Entities
{
    public class Contact : BaseEntity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsStatus { get; set; }

        public ICollection<Notification> Notification { get; set; }
    }
}
