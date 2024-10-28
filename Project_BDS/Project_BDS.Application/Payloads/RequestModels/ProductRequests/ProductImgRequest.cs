using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.RequestModels.ProductRequests
{
    public class ProductImgRequest
    {
        public int ProductId { get; set; }
        public string LinkImg { get; set; }
        public string Description { get; set; }
    }
}
