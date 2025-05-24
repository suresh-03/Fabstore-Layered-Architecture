using Fabstore.Domain.Interfaces.ICart;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FabstoreWebApplication.Controllers;

[Authorize]
public class CartController : Controller
    {

    private readonly ILogger<CartController> _logger;
    private readonly ICartService _cartService;

    public CartController(ILogger<CartController> logger, ICartService cartService)
        {
        _logger = logger;
        _cartService = cartService;
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

            var serviceResponse = await _cartService.GetCartItemsAsync(userIdentity);

            return View(serviceResponse.Data);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching cart items.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while getting cart items.", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }

        }

    [HttpGet]
    [Route("api/cart/add")]
    public async Task<IActionResult> AddToCart([FromQuery] int variantId)
        {
        try
            {
            var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!IsUserAuthenticated(userIdentity))
                {
                return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                }

            var serviceResponse = await _cartService.AddToCartAsync(userIdentity, variantId);

            _logger.LogInformation(serviceResponse.Message);
            return ResponseFilter.HandleResponse(serviceResponse);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while adding to cart.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while adding cart item", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }

    [HttpGet]
    [Route("api/cart/remove")]
    public async Task<IActionResult> RemoveFromCart(int variantId)
        {
        try
            {
            var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!IsUserAuthenticated(userIdentity))
                {
                return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                }

            var serviceResponse = await _cartService.RemoveFromCartAsync(userIdentity, variantId);

            _logger.LogInformation(serviceResponse.Message);
            return ResponseFilter.HandleResponse(serviceResponse);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while removing from cart.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while removing cart item", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }


    [HttpGet]
    [Route("api/cart/count")]
    public async Task<IActionResult> GetCartCount()
        {
        try
            {
            var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!IsUserAuthenticated(userIdentity))
                {
                return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                }

            var serviceResponse = await _cartService.GetCartCountAsync(userIdentity);
            return ResponseFilter.HandleResponse(serviceResponse);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching cart count.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while getting cart count", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }



    [HttpGet]
    [Route("api/cart/exists")]
    public async Task<IActionResult> ItemExistsInCart([FromQuery] int variantId)
        {
        try
            {
            var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!IsUserAuthenticated(userIdentity))
                {
                return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                }
            var serviceResponse = await _cartService.CartExistsAsync(userIdentity, variantId);
            return ResponseFilter.HandleResponse(serviceResponse);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching cart count.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while checking cart item existence.", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }

    [HttpPost]
    [Route("api/cart/updatequantity")]
    public async Task<IActionResult> UpdateCartQuantity([FromBody] CartQuantityModel cartModel)
        {
        try
            {
            var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!IsUserAuthenticated(userIdentity))
                {
                return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                }
            var serviceResponse = await _cartService.UpdateCartQuantity(userIdentity, cartModel.CartId, cartModel.Quantity);
            return ResponseFilter.HandleResponse(serviceResponse);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while updating cart count.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while updating cart quantity.", HttpStatusCode.INTERNAL_SERVER_ERROR);
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

public class CartQuantityModel
    {
    public int CartId { get; set; } = 0;
    public int Quantity { get; set; } = 0;
    }


