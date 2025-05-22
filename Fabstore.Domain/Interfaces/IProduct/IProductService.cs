using Fabstore.Domain.Models;

namespace Fabstore.Domain.Interfaces.IProduct
    {
    public interface IProductService
        {
        public Task<List<Product>> GetProductsAsync(string? categoy);

        public Task<(bool Success, string? Message, Product? Product)> GetProductDetailsAsync(string? category, int id);

        public Task<(bool Success, string Message, List<Product>? SearchedProducts)> GetSearchedProductsAsync(string query);

        public Task<(bool Success, string Message, Dictionary<string, List<string>> Categories)> GetCategoriesAsync();

        }
    }
