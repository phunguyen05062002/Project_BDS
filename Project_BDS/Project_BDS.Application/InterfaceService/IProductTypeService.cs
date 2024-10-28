using Project_BDS.Application.Payloads.RequestModels.ProductRequests;
using Project_BDS.Application.Payloads.Response_Models.DataProduct;
using Project_BDS.Application.Payloads.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Application.InterfaceService
{
    public interface IProductTypeService
    {
        Task<ResponseObject<DataResponse_ProductType>> AddProductTypeAsync(Request_ProductType request);
        Task<ResponseObject<DataResponse_ProductType>> UpdateProductTypeAsync(int id, Request_ProductType request);
        Task<ResponseObject<bool>> DeleteProductTypeAsync(int id);
        Task<ResponseObject<IEnumerable<DataResponse_ProductType>>> GetAllProductTypesAsync();
    }
}
