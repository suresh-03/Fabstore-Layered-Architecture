using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IWishlist;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Service;

// Service implementation for wishlist-related business logic
public class WishlistService : IWishlistService
    {

    // Repository for wishlist data access
    private readonly IWishlistRepository _wishlistRepository;
    // Factory for creating standardized service responses
    private readonly IServiceResponseFactory _responseFactory;

    // Constructor with dependency injection for repository and response factory
    public WishlistService(IWishlistRepository wishlistRepository, IServiceResponseFactory responseFactory)
        {
        _wishlistRepository = wishlistRepository;
        _responseFactory = responseFactory;
        }

    // Adds a product variant to the user's wishlist
    public async Task<IServiceResponse> AddToWishlistAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);

            // Check if the wishlist item already exists
            var wishlistItem = await _wishlistRepository.GetWishlistItemAsync(userId, productVariantId);

            if (wishlistItem != null && !wishlistItem.IsDeleted)
                {
                // Item already exists in wishlist and is not deleted
                return _responseFactory.CreateResponse(false, "Product Already Exists in the Wishlist", ActionType.Conflict);
                }

            if (wishlistItem != null && wishlistItem.IsDeleted)
                {
                // Restore previously deleted wishlist item
                await _wishlistRepository.AddToWishlistAsync(wishlistItem);
                }
            else
                {
                // Add new wishlist item
                Wishlist wishlist = new Wishlist
                    {
                    VariantID = productVariantId,
                    UserID = userId
                    };

                await _wishlistRepository.AddToWishlistAsync(wishlist);
                }
            return _responseFactory.CreateResponse(true, "Product Added to Wishlist", ActionType.Created);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while adding to wishlist.", ex);
            }
        }

    // Retrieves all wishlist items for the user
    public async Task<IServiceResponse<List<Wishlist>>> GetWishlistItemsAsync(string userIdentity)
        {
        try
            {
            int userId = int.Parse(userIdentity);
            var wishlistItems = await _wishlistRepository.GetWishlistItemsAsync(userId);
            if (wishlistItems == null || wishlistItems.Count == 0)
                {
                // No items found in wishlist
                return _responseFactory.CreateResponse<List<Wishlist>>(false, "No Items in Wishlist", ActionType.NotFound, null);
                }
            return _responseFactory.CreateResponse<List<Wishlist>>(true, "Items in Wishlist", ActionType.Retrieved, wishlistItems);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while retrieving wishlist items.", ex);
            }
        }

    // Removes a product variant from the user's wishlist
    public async Task<IServiceResponse> RemoveFromWishlistAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);
            var wishlistItem = await _wishlistRepository.GetWishlistItemAsync(userId, productVariantId);
            if (wishlistItem != null)
                {
                await _wishlistRepository.RemoveFromWishlistAsync(wishlistItem);
                return _responseFactory.CreateResponse(true, "Product Removed from Wishlist", ActionType.Deleted);
                }

            // Item not found in wishlist
            return _responseFactory.CreateResponse(false, "Product Not Found in Wishlist", ActionType.NotFound);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while removing wishlist item", ex);
            }
        }

    // Checks if a product variant exists in the user's wishlist
    public async Task<IServiceResponse<bool>> WishlistItemExistsAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);
            var wishlistItem = await _wishlistRepository.GetWishlistItemAsync(userId, productVariantId);
            if (wishlistItem == null || wishlistItem.IsDeleted)
                {
                // Item does not exist or is deleted
                return _responseFactory.CreateResponse<bool>(true, "Product not exists in the Wishlist", ActionType.Retrieved, false);
                }
            return _responseFactory.CreateResponse<bool>(true, "Product exists in the Wishlist", ActionType.Retrieved, true);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while checking wishlist existence.", ex);
            }
        }
    }