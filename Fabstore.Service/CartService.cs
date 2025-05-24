using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.ICart;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;
using Fabstore.Service.ResponseFormat;

namespace Fabstore.Service;

public class CartService : ICartService
    {
    private readonly ICartRepository _repo;

    public CartService(ICartRepository repo)
        {
        _repo = repo;
        }

    public async Task<IServiceResponse> AddToCartAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);

            var cartItem = await _repo.GetCartItemAsync(userId, productVariantId);

            if (cartItem != null && !cartItem.IsDeleted)
                {
                return new ServiceResponse(false, "Product Already Exists in the Cart", ActionType.Conflict);
                }

            if (cartItem != null && cartItem.IsDeleted)
                {
                await _repo.AddToCartAsync(cartItem);
                }
            else
                {
                // Create and add cart to both context and variant's collection
                Cart cart = new Cart
                    {
                    VariantID = productVariantId,
                    UserID = userId
                    };

                await _repo.AddToCartAsync(cart);
                }
            return new ServiceResponse(true, "Product Added to Cart", ActionType.Created);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while adding to cart.", ex);
            }
        }

    public async Task<IServiceResponse<bool>> CartExistsAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);
            var cartItem = await _repo.GetCartItemAsync(userId, productVariantId);
            if (cartItem == null || cartItem.IsDeleted)
                {
                return new ServiceResponse<bool>(true, "Product not exists in the Cart", ActionType.Retrieved, false);
                }
            return new ServiceResponse<bool>(true, "Product exists in the Cart", ActionType.Retrieved, true);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while checking cart existence.", ex);
            }
        }

    public async Task<IServiceResponse<int>> GetCartCountAsync(string userIdentity)
        {
        try
            {
            var userId = int.Parse(userIdentity);
            var cartCount = await _repo.GetCartCountAsync(userId);
            if (cartCount == 0)
                {
                return new ServiceResponse<int>(true, "No Items in Cart", ActionType.Retrieved, 0);
                }
            return new ServiceResponse<int>(true, "Items in Cart", ActionType.Retrieved, cartCount);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while retrieving cart count.", ex);
            }
        }

    public async Task<IServiceResponse<List<Cart>>> GetCartItemsAsync(string userIdentity)
        {
        try
            {
            int userId = int.Parse(userIdentity);
            var cartItems = await _repo.GetCartItemsAsync(userId);
            if (cartItems == null || cartItems.Count == 0)
                {
                return new ServiceResponse<List<Cart>>(false, "No Items in Cart", ActionType.NotFound, null);
                }
            return new ServiceResponse<List<Cart>>(true, "Items in Cart", ActionType.Retrieved, cartItems);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while retrieving cart items.", ex);
            }
        }

    public async Task<IServiceResponse> RemoveFromCartAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);
            var cartItem = await _repo.GetCartItemAsync(userId, productVariantId);
            if (cartItem != null)
                {
                await _repo.RemoveFromCartAsync(cartItem);
                return new ServiceResponse(true, "Product Removed from Cart", ActionType.Deleted);
                }

            return new ServiceResponse(false, "Product Not Found in Cart", ActionType.NotFound);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while removing cart item", ex);
            }
        }

    public async Task<IServiceResponse> UpdateCartQuantity(string userIdentity, int cartId, int quantity)
        {
        try
            {
            int userId = int.Parse(userIdentity);
            if (quantity <= 0)
                {
                return new ServiceResponse(false, "Quantity should be Greater than 0", ActionType.ValidationError);
                }
            await _repo.UpdateCartQuantity(userId, cartId, quantity);
            return new ServiceResponse(true, "Quantity Updated", ActionType.Updated);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while updating cart quantity.", ex);
            }
        }

    }



