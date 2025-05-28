using Fabstore.Domain.Interfaces.ICart;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FabstoreWebApplication.Controllers;

// Controller for handling cart-related actions for authenticated users
[Authorize]
public class CartController : Controller
    {

    // Logger for logging cart events and errors
    private readonly ILogger<CartController> _logger;
    // Service for cart-related business logic
    private readonly ICartService _cartService;

    // Constructor with dependency injection
    public CartController(ILogger<CartController> logger, ICartService cartService)
        {
        _logger = logger;
        _cartService = cartService;
        }

    // Displays the cart page with the user's cart items
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

            // Fetch cart items for the user
            var serviceResponse = await _cartService.GetCartItemsAsync(userIdentity);

            return View(serviceResponse.Data);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while fetching cart items.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while getting cart items.", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }

        }

    // Adds a product variant to the user's cart
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

    // Removes a product variant from the user's cart
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

    // Gets the count of items in the user's cart
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

    // Checks if a product variant exists in the user's cart
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
            _logger.LogError(ex, "Error occurred while fetching cart item existence.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while checking cart item existence.", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }

    // Updates the quantity of a specific cart item for the user
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

// Model for updating cart item quantity
public class CartQuantityModel
    {
    public int CartId { get; set; } = 0;
    public int Quantity { get; set; } = 0;
    }