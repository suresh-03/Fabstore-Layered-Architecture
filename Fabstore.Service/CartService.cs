using Fabstore.Domain.Interfaces.ICart;
using Fabstore.Domain.Models;

namespace Fabstore.Service;

public class CartService : ICartService
    {
    private readonly ICartRepository _repo;

    public CartService(ICartRepository repo)
        {
        _repo = repo;
        }

    public async Task<(bool Success, string Message)> AddToCartAsync(string userIdentity, int productVariantId)
        {
        var userId = int.Parse(userIdentity);

        var cartItem = await _repo.GetCartItemAsync(userId, productVariantId);

        if (cartItem != null && !cartItem.IsDeleted)
            {
            return (false, "Product Already Exists in the Cart");
            }

        if (cartItem != null && cartItem.IsDeleted)
            {
            cartItem.IsDeleted = false;
            await _repo.SaveDbChangesAsync();
            return (true, "Product Added to Cart");
            }

        // Create and add cart to both context and variant's collection
        Cart cart = new Cart
            {
            VariantID = productVariantId,
            UserID = userId
            };

        await _repo.AddToCartAsync(cart);
        return (true, "Product Added to Cart");
        }

    public async Task<(bool Success, string Message, bool CartExists)> CartExistsAsync(string userIdentity, int productVariantId)
        {
        var userId = int.Parse(userIdentity);
        var cartItem = await _repo.GetCartItemAsync(userId, productVariantId);
        if (cartItem == null)
            {
            return (false, "Product not exists in the Cart", false);
            }
        return (true, "Product exists in the Cart", true);
        }

    public async Task<(bool Success, string Message, int CartCount)> GetCartCountAsync(string userIdentity)
        {
        var userId = int.Parse(userIdentity);
        var cartCount = await _repo.GetCartCountAsync(userId);
        if (cartCount == 0)
            {
            return (false, "No Items in Cart", 0);
            }
        return (true, "Items in Cart", cartCount);
        }

    public async Task<List<Cart>> GetCartItemsAsync(string userIdentity)
        {
        int userId = int.Parse(userIdentity);
        return await _repo.GetCartItemsAsync(userId);
        }

    public async Task<(bool Success, string Message)> RemoveFromCartAsync(string userIdentity, int productVariantId)
        {
        var userId = int.Parse(userIdentity);
        var cartItem = await _repo.GetCartItemAsync(userId, productVariantId);
        if (cartItem != null)
            {
            cartItem.IsDeleted = true;
            cartItem.DeletedAt = DateTime.UtcNow;
            await _repo.RemoveFromCartAsync();
            return (true, "Product Removed from Cart");
            }

        return (false, "Product Not Found in Cart");
        }

    }



