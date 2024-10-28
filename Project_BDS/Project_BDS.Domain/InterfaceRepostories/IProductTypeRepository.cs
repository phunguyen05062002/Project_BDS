using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.InterfaceRepostories
{
    public interface IProductTypeRepository
    {
        Task<IEnumerable<ProductType>> GetAllAsync();
        Task<ProductType> GetByIdAsync(int id);
        Task AddAsync(ProductType entity);
        Task UpdateAsync(ProductType entity);
        Task DeleteAsync(ProductType entity);
        Task SaveChangesAsync();
    }
}
