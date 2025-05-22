using Fabstore.DataAccess.Database;
using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess
    {
    public class ProductRepository : IProductRepository
        {

        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
            {
            _context = context;
            }



        public async Task<List<Product>> GetProductsAsync(string? category)
            {
            if (string.IsNullOrEmpty(category))
                {
                return await _context.Products
                         .Include(p => p.Brand)
                         .Include(p => p.Category)
                         .Include(p => p.Variants)
                             .ThenInclude(v => v.Images).ToListAsync();

                }
            else
                {
                return await _context.Products
                         .Include(p => p.Brand)
                         .Include(p => p.Category)
                         .Include(p => p.Variants)
                             .ThenInclude(v => v.Images)
                         .Where(p => p.Category.CategoryName == category).ToListAsync();

                }
            }

        public async Task<Product> GetProductDetailsAsync(string? category, int id)
            {
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images)
                .Where(p => p.ProductID == id);

            if (!string.IsNullOrEmpty(category))
                {
                query = query.Where(p => p.Category.CategoryName == category);
                }

            return await query.FirstOrDefaultAsync();
            }

        public async Task<List<Product>> GetSearchedProductsAsync()
            {
            return await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category).ThenInclude(c => c.ParentCategory)
            .Include(p => p.Variants).ThenInclude(v => v.Images)
            .ToListAsync();
            }

        public async Task<List<Category>> GetCategoriesAsync()
            {

            return await _context.Categories
                .Where(c => c.ParentCategoryID != null)
                .Include(c => c.SubCategories)
                .ToListAsync();
            }

        public async Task<bool> SaveDbChangesAsync()
            {
            await _context.SaveChangesAsync();
            return true;
            }
        }
    }
