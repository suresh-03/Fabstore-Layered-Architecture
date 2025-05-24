using Fabstore.Domain.Models;

namespace Fabstore.Domain.Interfaces.ICart
    {
    public interface ICartRepository
        {
        public Task<List<Cart>> GetCartItemsAsync(int userId);

        public Task<Cart> GetCartItemAsync(int userId, int productVariantId);

        public Task<bool> AddToCartAsync(Cart cartItem);

        public Task<bool> RemoveFromCartAsync(Cart cartItem);

        public Task<int> GetCartCountAsync(int userId);

        public Task UpdateCartQuantity(int userId, int cartId, int quantity);
        }
    }
