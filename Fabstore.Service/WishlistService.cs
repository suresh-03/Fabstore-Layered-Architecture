using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IWishlist;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Service;

public class WishlistService : IWishlistService
    {

    private readonly IWishlistRepository _wishlistRepository;
    private readonly IServiceResponseFactory _responseFactory;

    public WishlistService(IWishlistRepository wishlistRepository, IServiceResponseFactory responseFactory)
        {
        _wishlistRepository = wishlistRepository;
        _responseFactory = responseFactory;
        }

    public async Task<IServiceResponse> AddToWishlistAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);

            var wishlistItem = await _wishlistRepository.GetWishlistItemAsync(userId, productVariantId);

            if (wishlistItem != null && !wishlistItem.IsDeleted)
                {
                return _responseFactory.CreateResponse(false, "Product Already Exists in the Wishlist", ActionType.Conflict);
                }

            if (wishlistItem != null && wishlistItem.IsDeleted)
                {
                await _wishlistRepository.AddToWishlistAsync(wishlistItem);
                }
            else
                {

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

    public async Task<IServiceResponse<List<Wishlist>>> GetWishlistItemsAsync(string userIdentity)
        {
        try
            {
            int userId = int.Parse(userIdentity);
            var wishlistItems = await _wishlistRepository.GetWishlistItemsAsync(userId);
            if (wishlistItems == null || wishlistItems.Count == 0)
                {
                return _responseFactory.CreateResponse<List<Wishlist>>(false, "No Items in Wishlist", ActionType.NotFound, null);
                }
            return _responseFactory.CreateResponse<List<Wishlist>>(true, "Items in Wishlist", ActionType.Retrieved, wishlistItems);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while retrieving wishlist items.", ex);
            }
        }

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

            return _responseFactory.CreateResponse(false, "Product Not Found in Wishlist", ActionType.NotFound);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while removing wishlist item", ex);
            }
        }

    public async Task<IServiceResponse<bool>> WishlistItemExistsAsync(string userIdentity, int productVariantId)
        {
        try
            {
            var userId = int.Parse(userIdentity);
            var wishlistItem = await _wishlistRepository.GetWishlistItemAsync(userId, productVariantId);
            if (wishlistItem == null || wishlistItem.IsDeleted)
                {
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

