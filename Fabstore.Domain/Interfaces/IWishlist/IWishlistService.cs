using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Domain.Interfaces.IWishlist;

public interface IWishlistService
    {
    public Task<IServiceResponse> AddToWishlistAsync(string userIdentity, int productVariantId);

    public Task<IServiceResponse> RemoveFromWishlistAsync(string userIdentity, int productVariantId);

    public Task<IServiceResponse<List<Wishlist>>> GetWishlistItemsAsync(string userIdentity);

    public Task<IServiceResponse<bool>> WishlistItemExistsAsync(string userIdentity, int productVariantId);
    }

