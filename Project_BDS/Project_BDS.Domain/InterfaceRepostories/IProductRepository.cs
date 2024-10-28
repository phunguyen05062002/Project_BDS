using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.InterfaceRepostories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<int> GetTotalCountAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<List<Product>> SearchByTypeAsync(int typeId, int pageIndex, int pageSize);
        Task<List<Product>> SearchByPriceAsync(double minPrice, double maxPrice, int pageIndex, int pageSize);
        Task<List<Product>> SearchByAddressAsync(string address, int pageIndex, int pageSize);
        Task<List<Product>> SearchByStartSellingAsync(int? startYear, int? endYear, int? startMonth, int? endMonth, int pageIndex, int pageSize);
        Task<Dictionary<string, double>> GetTotalPriceByTypeAsync();
        Task<bool> ExistsAsync(int productId);
    }
}
