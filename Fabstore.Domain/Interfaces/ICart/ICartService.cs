using Fabstore.Domain.Models;

namespace Fabstore.Domain.Interfaces.ICart
    {
    public interface ICartService
        {

        public Task<List<Cart>> GetCartItemsAsync(string userIdentity);

        public Task<(bool Success, string Message)> AddToCartAsync(string userIdentity, int productVariantId);

        public Task<(bool Success, string Message)> RemoveFromCartAsync(string userIdentity, int productVariantId);

        public Task<(bool Success, string Message, int CartCount)> GetCartCountAsync(string userIdentity);

        public Task<(bool Success, string Message, bool CartExists)> CartExistsAsync(string userIdentity, int productVariantId);

        }
    }
