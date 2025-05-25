using Fabstore.Domain.Interfaces.IWishlist;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fabstore.WebApplication.Controllers
    {
    [Authorize]
    public class WishlistController : Controller
        {
        private readonly ILogger<WishlistController> _logger;
        private readonly IWishlistService _wishlistService;

        public WishlistController(ILogger<WishlistController> logger, IWishlistService wishlistService)
            {
            _logger = logger;
            _wishlistService = wishlistService;
            }

        public async Task<IActionResult> Index()
            {
            try
                {
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!IsUserAuthenticated(userIdentity))
                    {
                    return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                    }

                var serviceResponse = await _wishlistService.GetWishlistItemsAsync(userIdentity);

                return View(serviceResponse.Data);
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while fetching wishlist items.");
                return ResponseFilter.HandleResponse(false, "Something went wrong while getting wishlist items.", HttpStatusCode.INTERNAL_SERVER_ERROR);
                }

            }

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
