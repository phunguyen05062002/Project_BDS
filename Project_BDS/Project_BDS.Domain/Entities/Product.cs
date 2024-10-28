using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string HostName { get; set; }
        public string HostPhoneNumber { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; } = true;
        public int TypeId { get; set; }
        public ProductType? Type { get; set; }
        public DateTime StartSelling { get; set; }
        public DateTime? UpdateTime { get; set; }

        public ICollection<ProductImg>? ProductImgs { get; set; }
        public ICollection<Contact>? Contacts { get; set; }
    }
}
