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
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.ProductImgs) 
                .ToListAsync();
        }


        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Products.CountAsync();
        }


        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductImgs) // Bao gồm các ảnh liên quan
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> SearchByTypeAsync(int typeId)
        {
            return await _context.Products
                .Where(p => p.TypeId == typeId)
                .Include(p => p.Type)
                .ToListAsync();
        }


        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Product>> SearchByTypeAsync(int typeId, int pageIndex, int pageSize)
        {
            return await _context.Products
                .Where(p => p.TypeId == typeId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchByPriceAsync(double minPrice, double maxPrice, int pageIndex, int pageSize)
        {
            return await _context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchByAddressAsync(string address, int pageIndex, int pageSize)
        {
            return await _context.Products
                .Where(p => p.Address.Contains(address))
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchByStartSellingAsync(int? startYear, int? endYear, int? startMonth, int? endMonth, int pageIndex, int pageSize)
        {
            IQueryable<Product> query = _context.Products;

            if (startYear.HasValue && endYear.HasValue)
            {
                query = query.Where(p => p.StartSelling.Year >= startYear.Value && p.StartSelling.Year <= endYear.Value);
            }

            if (startMonth.HasValue && endMonth.HasValue)
            {
                if (startYear.HasValue)
                {
                    query = query.Where(p => p.StartSelling.Year == startYear.Value && p.StartSelling.Month >= startMonth.Value && p.StartSelling.Month <= endMonth.Value);
                }
                else if (endYear.HasValue)
                {
                    query = query.Where(p => p.StartSelling.Year == endYear.Value && p.StartSelling.Month >= startMonth.Value && p.StartSelling.Month <= endMonth.Value);
                }
                else
                {
                    query = query.Where(p => p.StartSelling.Month >= startMonth.Value && p.StartSelling.Month <= endMonth.Value);
                }
            }

            return await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<Dictionary<string, double>> GetTotalPriceByTypeAsync()
        {
            // Lấy danh sách tất cả các loại bất động sản
            var productTypes = await _context.ProductTypes.ToListAsync();

            // Tính tổng giá của từng loại bất động sản
            var totalPrices = await _context.Products
                .GroupBy(p => p.TypeId)
                .Select(group => new
                {
                    ProductTypeId = group.Key,
                    TotalPrice = group.Sum(p => p.Price)
                })
                .ToListAsync();

            // Tạo một Dictionary với tất cả các loại bất động sản và tổng giá của chúng
            var result = productTypes.ToDictionary(
                pt => pt.Name,
                pt => totalPrices.FirstOrDefault(tp => tp.ProductTypeId == pt.Id)?.TotalPrice ?? 0
            );

            return result;
        }
        public async Task<bool> ExistsAsync(int productId)
        {
            return await _context.Products.AnyAsync(p => p.Id == productId);
        }
    }
}
