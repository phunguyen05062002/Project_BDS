using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.DataContexts;
using Project_BDS.Domain.Entities;
using Project_BDS.Domain.InterfaceRepostories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.ImplementRepostories
{
    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly AppDbContext _context;

        public ProductTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductType>> GetAllAsync()
        {
            return await _context.ProductTypes.ToListAsync();
        }

        public async Task<ProductType> GetByIdAsync(int id)
        {
            return await _context.ProductTypes.FindAsync(id);
        }

        public async Task AddAsync(ProductType entity)
        {
            await _context.ProductTypes.AddAsync(entity);
        }

        public async Task UpdateAsync(ProductType entity)
        {
            _context.ProductTypes.Update(entity);
        }

        public async Task DeleteAsync(ProductType entity)
        {
            _context.ProductTypes.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
