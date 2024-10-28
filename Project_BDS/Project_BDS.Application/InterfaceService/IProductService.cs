using Project_BDS.Application.Helper;
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
    public interface IProductService
    {
        Task<ResponseObject<DataResponseProduct>> AddProductAsync(RequestProduct requestProduct);
        Task<ResponseObject<DataResponseProduct>> UpdateProductAsync(int productId, RequestProduct requestProduct);
        Task<ResponseObject<bool>> DeleteProductAsync(int productId);
        //Task<ResponseObject<PagedResult<DataResponseProduct>>> GetAllProductsAsync(int pageIndex, int pageSize);
        Task<ResponseObject<IEnumerable<DataResponseProduct>>> GetAllProductsAsync();
        Task<ResponseObject<DataResponseProduct>> GetProductByIdAsync(int productId);
        Task<ResponseObject<IEnumerable<DataResponseProduct>>> SearchProductsByTypeAsync(int typeId, int pageIndex, int pageSize);
        Task<ResponseObject<IEnumerable<DataResponseProduct>>> SearchProductsByPriceAsync(double minPrice, double maxPrice, int pageIndex, int pageSize);
        Task<ResponseObject<IEnumerable<DataResponseProduct>>> SearchProductsByAddressAsync(string address, int pageIndex, int pageSize);
        Task<ResponseObject<IEnumerable<DataResponseProduct>>> SearchProductsByStartSellingAsync(int? startYear, int? endYear, int? startMonth, int? endMonth, int pageIndex, int pageSize);
        Task<Dictionary<string, double>> GetTotalPriceByTypeAsync();
    }
}
