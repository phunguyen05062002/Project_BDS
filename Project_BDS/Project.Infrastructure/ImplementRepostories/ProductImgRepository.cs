using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.DataContexts;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System.Threading.Tasks;

namespace Project.Infrastructure.ImplementRepostories
{
    public class ProductImgRepository : IProductImgRepository
    {
        private readonly AppDbContext _context;

        public ProductImgRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductImg productImg)
        {
            await _context.ProductImgs.AddAsync(productImg);
            await _context.SaveChangesAsync();
        }
        public async Task<List<ProductImg>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductImgs.Where(img => img.ProductId == productId).ToListAsync();
        }

        public async Task<ProductImg> GetByIdAsync(int imageId)
        {
            return await _context.ProductImgs.FindAsync(imageId);
        }

        public async Task UpdateAsync(ProductImg productImg)
        {
            _context.ProductImgs.Update(productImg);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductImg productImg)
        {
            _context.ProductImgs.Remove(productImg);
            await _context.SaveChangesAsync();
        }
        public async Task<List<ProductImg>> GetAllAsync()
        {
            return await _context.ProductImgs.ToListAsync();
        }
    }
}
