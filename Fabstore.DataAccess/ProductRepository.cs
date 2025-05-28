using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess
    {
    // Repository for product and category data access
    public class ProductRepository : IProductRepository
        {

        // Database context for accessing data
        private readonly AppDbContext _context;

        // Constructor with dependency injection for the database context
        public ProductRepository(AppDbContext context)
            {
            _context = context;
            }

        // Retrieves a list of products, optionally filtered by category name
        public async Task<List<Product>> GetProductsAsync(string? category)
            {
            try
                {
                if (string.IsNullOrEmpty(category))
                    {
                    // Return all products with related data
                    return await _context.Products
                             .Include(p => p.Brand)
                             .Include(p => p.Category)
                             .Include(p => p.Variants)
                                 .ThenInclude(v => v.Images)
                             .Include(p => p.Reviews)
                             .ToListAsync();
                    }
                else
                    {
                    // Return products filtered by category name with related data
                    return await _context.Products
                             .Include(p => p.Brand)
                             .Include(p => p.Category)
                             .Include(p => p.Variants)
                                 .ThenInclude(v => v.Images)
                             .Include(p => p.Reviews)
                             .Where(p => p.Category.CategoryName == category).ToListAsync();
                    }
                }
            catch (Exception ex)
                {
                // Wrap and throw a custom database exception
                throw new DatabaseException("An error occurred while retrieving products.", ex);
                }
            }

        // Retrieves the details of a specific product by ID, optionally filtered by category name
        public async Task<Product> GetProductDetailsAsync(string? category, int id)
            {
            try
                {
                // Build query for product details with related data
                var query = _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Variants)
                        .ThenInclude(v => v.Images)
                    .Include(p => p.Reviews)
                    .Where(p => p.ProductID == id);

                if (!string.IsNullOrEmpty(category))
                    {
                    // Further filter by category name if provided
                    query = query.Where(p => p.Category.CategoryName == category);
                    }

                return await query.FirstOrDefaultAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("An error occurred while retrieving product details.", ex);
                }
            }

        // Retrieves all products with related brand, category, variant, and review data
        public async Task<List<Product>> GetSearchedProductsAsync()
            {
            try
                {
                return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category).ThenInclude(c => c.ParentCategory)
                .Include(p => p.Variants).ThenInclude(v => v.Images)
                .Include(p => p.Reviews)
                .ToListAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("An error occurred while retrieving searched products.", ex);
                }
            }

        // Retrieves categories that have a parent category, including their subcategories
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