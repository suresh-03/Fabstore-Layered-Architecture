using AutoMapper;
using Fabstore.Domain.Interfaces.ICart;
using FabstoreWebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FabstoreWebApplication.Controllers
    {
    [Authorize]
    public class CartController : Controller
        {

        private readonly ILogger<CartController> _logger;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;

        public CartController(ILogger<CartController> logger, IMapper mapper, ICartService cartService)
            {
            _logger = logger;
            _mapper = mapper;
            _cartService = cartService;
            }

        public async Task<IActionResult> Index()
            {
            try
                {
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdentity))
                    {
                    _logger.LogWarning("Fetch Cart: User is not Authenticated");
                    return Unauthorized(new { message = "User is not Authenticated" });
                    }

                var cartItems = await _cartService.GetCartItemsAsync(userIdentity);
                List<CartView> cartView = _mapper.Map<List<CartView>>(cartItems);

                return View(cartView);
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while fetching cart items.");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching cart items." });
                }

            }

        [HttpGet]
        [Route("api/cart/add")]
        public async Task<IActionResult> AddToCart([FromQuery] int variantId)
            {
            try
                {
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdentity))
                    {
                    _logger.LogWarning("AddToCart failed: User is not authenticated.");
                    return Unauthorized(new { message = "User is not authenticated." });
                    }

                var result = await _cartService.AddToCartAsync(userIdentity, variantId);

                if (!result.Success)
                    {
                    _logger.LogWarning($"AddToCart failed: {result.Message}");
                    return BadRequest(new { success = false, message = result.Message });
                    }

                _logger.LogInformation($"Product variant {variantId} added to cart for user {userIdentity}.");
                return Json(new { success = true, variantId });
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while adding to cart.");
                return StatusCode(500, new { success = false, message = "An error occurred while adding to cart.", variantId });
                }
            }

        [HttpGet]
        [Route("api/cart/remove")]
        public async Task<IActionResult> RemoveFromCart(int variantId)
            {
            try
                {
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdentity))
                    {
                    _logger.LogWarning("RemoveFromCart failed: User is not authenticated.");
                    return Unauthorized(new { message = "User is not authenticated." });
                    }

                var result = await _cartService.RemoveFromCartAsync(userIdentity, variantId);

                _logger.LogInformation($"Product variant {variantId} removed from cart for user {userIdentity}.");
                return Json(new { success = true, variantId });
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while removing from cart.");
                return StatusCode(500, new { success = false, message = "An error occurred while removing from cart." });
                }
            }


        [HttpGet]
        [Route("api/cart/count")]
        public async Task<IActionResult> GetCartCount()
            {
            try
                {
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdentity))
                    {
                    _logger.LogWarning("GetCartCount: User is not authenticated.");
                    return Json(new { cartItemsCount = 0 });
                    }

                var result = await _cartService.GetCartCountAsync(userIdentity);
                var cartItemsCount = result.CartCount;
                return Json(new { cartItemsCount });
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while fetching cart count.");
                return StatusCode(500, new { cartItemsCount = 0, message = "An error occurred." });
                }
            }



        [HttpGet]
        [Route("api/cart/exists")]
        public async Task<IActionResult> ItemExistsInCart([FromQuery] int variantId)
            {
            try
                {
                var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdentity))
                    {
                    _logger.LogWarning("RemoveFromCart failed: User is not authenticated.");
                    return Unauthorized(new { message = "User is not authenticated." });
                    }
                var result = await _cartService.CartExistsAsync(userIdentity, variantId);
                var itemExists = result.CartExists;
                return Json(new { itemExists });
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Error occurred while fetching cart count.");
                return StatusCode(500, new { cartItemsCount = 0, message = "An error occurred." });
                }
            }

        }
    }
