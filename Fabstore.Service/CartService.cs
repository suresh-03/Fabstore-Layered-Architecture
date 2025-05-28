using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.ICart;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Service;

// Service implementation for cart-related business logic
public class CartService : ICartService
    {
    // Repository for cart data access
    private readonly ICartRepository _cartRepository;
    // Factory for creating standardized service responses
    private readonly IServiceResponseFactory _responseFactory;

    // Constructor with dependency injection for repository and response factory
    public CartService(ICartRepository cartRepository, IServiceResponseFactory responseFactory)
        {
        _cartRepository = cartRepository;
        _responseFactory = responseFactory;
        }

    // Adds a product variant to the user's cart
    public async Task<IServiceResponse> AddToCartAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);

            // Check if the cart item already exists
            var cartItem = await _cartRepository.GetCartItemAsync(userId, productVariantId);

            if (cartItem != null && !cartItem.IsDeleted)
                {
                // Item already exists in cart and is not deleted
                return _responseFactory.CreateResponse(false, "Product Already Exists in the Cart", ActionType.Conflict);
                }

            if (cartItem != null && cartItem.IsDeleted)
                {
                // Restore previously deleted cart item
                await _cartRepository.AddToCartAsync(cartItem);
                }
            else
                {
                // Add new cart item
                Cart cart = new Cart
                    {
                    VariantID = productVariantId,
                    UserID = userId
                    };

                await _cartRepository.AddToCartAsync(cart);
                }
            return _responseFactory.CreateResponse(true, "Product Added to Cart", ActionType.Created);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while adding to cart.", ex);
            }
        }

    // Checks if a product variant exists in the user's cart
    public async Task<IServiceResponse<bool>> CartExistsAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);
            var cartItem = await _cartRepository.GetCartItemAsync(userId, productVariantId);
            if (cartItem == null || cartItem.IsDeleted)
                {
                // Item does not exist or is deleted
                return _responseFactory.CreateResponse<bool>(true, "Product not exists in the Cart", ActionType.Retrieved, false);
                }
            return _responseFactory.CreateResponse<bool>(true, "Product exists in the Cart", ActionType.Retrieved, true);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while checking cart existence.", ex);
            }
        }

    // Gets the count of items in the user's cart
    public async Task<IServiceResponse<int>> GetCartCountAsync(string userIdentity)
        {
        try
            {
            var userId = int.Parse(userIdentity);
            var cartCount = await _cartRepository.GetCartCountAsync(userId);
            if (cartCount == 0)
                {
                // No items in cart
                return _responseFactory.CreateResponse<int>(true, "No Items in Cart", ActionType.Retrieved, 0);
                }
            return _responseFactory.CreateResponse<int>(true, "Items in Cart", ActionType.Retrieved, cartCount);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while retrieving cart count.", ex);
            }
        }

    // Retrieves all cart items for the user
    public async Task<IServiceResponse<List<Cart>>> GetCartItemsAsync(string userIdentity)
        {
        try
            {
            int userId = int.Parse(userIdentity);
            var cartItems = await _cartRepository.GetCartItemsAsync(userId);
            if (cartItems == null || cartItems.Count == 0)
                {
                // No items found in cart
                return _responseFactory.CreateResponse<List<Cart>>(false, "No Items in Cart", ActionType.NotFound, null);
                }
            return _responseFactory.CreateResponse<List<Cart>>(true, "Items in Cart", ActionType.Retrieved, cartItems);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while retrieving cart items.", ex);
            }
        }

    // Removes a product variant from the user's cart
    public async Task<IServiceResponse> RemoveFromCartAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);
            var cartItem = await _cartRepository.GetCartItemAsync(userId, productVariantId);
            if (cartItem != null)
                {
                await _cartRepository.RemoveFromCartAsync(cartItem);
                return _responseFactory.CreateResponse(true, "Product Removed from Cart", ActionType.Deleted);
                }

            // Item not found in cart
            return _responseFactory.CreateResponse(false, "Product Not Found in Cart", ActionType.NotFound);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while removing cart item", ex);
            }
        }

    // Updates the quantity of a specific cart item for the user
    public async Task<IServiceResponse> UpdateCartQuantity(string userIdentity, int cartId, int quantity)
        {
        try
            {
            int userId = int.Parse(userIdentity);
            if (quantity <= 0)
                {
                // Quantity validation failed
                return _responseFactory.CreateResponse(false, "Quantity should be Greater than 0", ActionType.ValidationError);
                }
            await _cartRepository.UpdateCartQuantity(userId, cartId, quantity);
            return _responseFactory.CreateResponse(true, "Quantity Updated", ActionType.Updated);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while updating cart quantity.", ex);
            }
        }

    }