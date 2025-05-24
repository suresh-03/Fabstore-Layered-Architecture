using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
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
            try
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
            catch (Exception ex)
                {
                throw new DatabaseException("An error occurred while retrieving products.", ex);
                }
            }

        public async Task<Product> GetProductDetailsAsync(string? category, int id)
            {
            try
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
            catch (Exception ex)
                {
                throw new DatabaseException("An error occurred while retrieving product details.", ex);
                }
            }

        public async Task<List<Product>> GetSearchedProductsAsync()
            {
            try
                {
                return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category).ThenInclude(c => c.ParentCategory)
                .Include(p => p.Variants).ThenInclude(v => v.Images)
                .ToListAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("An error occurred while retrieving searched products.", ex);
                }
            }

        public async Task<List<Category>> GetCategoriesAsync()
            {
            try
                {
                return await _context.Categories
                    .Where(c => c.ParentCategoryID != null)
                    .Include(c => c.SubCategories)
                    .ToListAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("An error occurred while retrieving categories.", ex);
                }
            }


        }
    }
