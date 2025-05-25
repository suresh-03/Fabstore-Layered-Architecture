using Fabstore.Domain.Models;

namespace Fabstore.Domain.Interfaces.IWishlist;

public interface IWishlistRepository
    {
    public Task AddToWishlistAsync(Wishlist wishlistItem);

    public Task RemoveFromWishlistAsync(Wishlist wishlistItem);

    public Task<List<Wishlist>> GetWishlistItemsAsync(int userId);

    public Task<Wishlist> GetWishlistItemAsync(int userId, int productVariantId);
    }

