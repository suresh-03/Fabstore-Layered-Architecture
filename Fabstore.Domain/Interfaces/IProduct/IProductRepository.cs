using Fabstore.Domain.Models;

namespace Fabstore.Domain.Interfaces.IProduct
    {
    public interface IProductRepository : IBaseRepository
        {
        public Task<List<Product>> GetProductsAsync(string? category);

        public Task<Product> GetProductDetailsAsync(string? category, int id);

        public Task<List<Product>> GetSearchedProductsAsync();

        public Task<List<Category>> GetCategoriesAsync();
        }
    }
