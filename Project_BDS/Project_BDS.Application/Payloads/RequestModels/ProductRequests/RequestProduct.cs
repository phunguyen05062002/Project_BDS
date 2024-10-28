using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.RequestModels.ProductRequests
{
    public class RequestProduct
    {
        public string HostName { get; set; }
        public string HostPhoneNumber { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public int TypeId { get; set; }
        public DateTime StartSelling { get; set; }
        public bool IsActive { get; set; }
        public List<ProductImgRequest> ProductImgs { get; set; } = new List<ProductImgRequest>();
    }
}
