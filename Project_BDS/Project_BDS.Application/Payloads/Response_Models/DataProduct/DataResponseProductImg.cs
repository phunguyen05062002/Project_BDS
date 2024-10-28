using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.Payloads.Response_Models.DataProduct
{
    public class DataResponseProductImg : DataResponseBase
    {
        public int ProductId { get; set; }
        public string LinkImg { get; set; }
        public string Description { get; set; }
    }
}
