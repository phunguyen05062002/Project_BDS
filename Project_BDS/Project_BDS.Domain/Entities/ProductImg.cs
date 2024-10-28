using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.Entities
{
    public class ProductImg : BaseEntity
    {
        public int ProductId {  get; set; }
        public Product? Product { get; set; }
        public string LinkImg {  get; set; }
        public string Description {  get; set; }
    }
}
