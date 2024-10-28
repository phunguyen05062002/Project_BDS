using Project_BDS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_BDS.Domain.InterfaceRepostories
{
    public interface IProductImgRepository
    {
        Task AddAsync(ProductImg productImg);
        Task<List<ProductImg>> GetByProductIdAsync(int productId);
        Task<ProductImg> GetByIdAsync(int imageId);
        Task UpdateAsync(ProductImg productImg);
        Task DeleteAsync(ProductImg productImg);
        Task<List<ProductImg>> GetAllAsync();
    }
}
