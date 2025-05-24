using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Domain.Interfaces.IProduct
    {
    public interface IProductService
        {
        public Task<IServiceResponse<List<Product>>> GetProductsAsync(string? categoy);

        public Task<IServiceResponse<Product>> GetProductDetailsAsync(string? category, int id);

        public Task<IServiceResponse<List<Product>>> GetSearchedProductsAsync(string query);

        public Task<IServiceResponse<Dictionary<string, List<string>>>> GetCategoriesAsync();

        }
    }
