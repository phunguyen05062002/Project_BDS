using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.Entities
{
    public class ProductType : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public ICollection<Product>? Products { get; set;}
    }
}
