using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Domain.Interfaces.ICart
    {
    public interface ICartService
        {

        public Task<IServiceResponse<List<Cart>>> GetCartItemsAsync(string userIdentity);

        public Task<IServiceResponse> AddToCartAsync(string userIdentity, int productVariantId);

        public Task<IServiceResponse> RemoveFromCartAsync(string userIdentity, int productVariantId);

        public Task<IServiceResponse<int>> GetCartCountAsync(string userIdentity);

        public Task<IServiceResponse<bool>> CartExistsAsync(string userIdentity, int productVariantId);

        public Task<IServiceResponse> UpdateCartQuantity(string userIdentity, int cartId, int quantity);

        }
    }
