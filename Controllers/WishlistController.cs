using Fabstore.Domain.Interfaces.IWishlist;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fabstore.WebApplication.Controllers
    {
    // Controller for handling wishlist-related actions for authenticated users
    [Authorize]
    public class WishlistController : Controller
        {
        // Logger for logging wishlist events and errors
        private readonly ILogger<WishlistController> _logger;
        // Service for wishlist-related business logic
        private readonly IWishlistService _wishlistService;

        // Constructor with dependency injection
        public WishlistController(ILogger<WishlistController> logger, IWishlistService wishlistService)
            {
            _logger = logger;
            _wishlistService = wishlistService;
            }

        // Displays the wishlist page with the user's wishlist items
        public async Task<IActionResult> Index()
            {
            try
                {
                // Get the user's identity from claims
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Check if the user is authenticated
                if (!IsUserAuthenticated(userIdentity))
                    {
                    return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                    }

                // Fetch wishlist items for the user
                var serviceResponse = await _wishlistService.GetWishlistItemsAsync(userIdentity);

                return View(serviceResponse.Data);
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while fetching wishlist items.");
                return ResponseFilter.HandleResponse(false, "Something went wrong while getting wishlist items.", HttpStatusCode.INTERNAL_SERVER_ERROR);
                }
            }

        // Adds a product variant to the user's wishlist
        [HttpGet]
        [Route("api/wishlist/add")]
        public async Task<IActionResult> AddToWishlist([FromQuery] int variantId)
            {
            try
                {
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!IsUserAuthenticated(userIdentity))
                    {
                    return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                    }

                var serviceResponse = await _wishlistService.AddToWishlistAsync(userIdentity, variantId);

                _logger.LogInformation(serviceResponse.Message);
                return ResponseFilter.HandleResponse(serviceResponse);
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while adding to wishlist.");
                return ResponseFilter.HandleResponse(false, "Something went wrong while adding wishlist item", HttpStatusCode.INTERNAL_SERVER_ERROR);
                }
            }

        // Removes a product variant from the user's wishlist
        [HttpGet]
        [Route("api/wishlist/remove")]
        public async Task<IActionResult> RemoveFromWishlist(int variantId)
            {
            try
                {
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!IsUserAuthenticated(userIdentity))
                    {
                    return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                    }

                var serviceResponse = await _wishlistService.RemoveFromWishlistAsync(userIdentity, variantId);

                _logger.LogInformation(serviceResponse.Message);
                return ResponseFilter.HandleResponse(serviceResponse);
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while removing from wishlist.");
                return ResponseFilter.HandleResponse(false, "Something went wrong while removing wishlist item", HttpStatusCode.INTERNAL_SERVER_ERROR);
                }
            }

        // Checks if a product variant exists in the user's wishlist
        [HttpGet]
        [Route("api/wishlist/exists")]
        public async Task<IActionResult> ItemExistsInWishlist([FromQuery] int variantId)
            {
            try
                {
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!IsUserAuthenticated(userIdentity))
                    {
                    return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                    }
                var serviceResponse = await _wishlistService.WishlistItemExistsAsync(userIdentity, variantId);
                return ResponseFilter.HandleResponse(serviceResponse);
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while fetching wishlist item existence.");
                return ResponseFilter.HandleResponse(false, "Something went wrong while checking wishlist item existence.", HttpStatusCode.INTERNAL_SERVER_ERROR);
                }
            }

        // Checks if the user is authenticated based on their identity
        private bool IsUserAuthenticated(string userIdentity)
            {
            if (string.IsNullOrEmpty(userIdentity))
                {
                _logger.LogWarning("User is not authenticated.");
                return false;
                }

            return true;
            }
        }
    }