using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.Response_Models.DataProduct
{
    public class DataResponseProduct : DataResponseBase
    {
        public string HostName { get; set; }
        public string HostPhoneNumber { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public bool? IsActive { get; set; }
        public int TypeId { get; set; }
        public DateTime StartSelling { get; set; }
        public DateTime? UpdateTime { get; set; }
        public List<DataResponseProductImg> ProductImgs { get; set; } = new List<DataResponseProductImg>();
    }
}
