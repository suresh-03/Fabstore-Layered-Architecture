using Fabstore.Domain.Interfaces.IReview;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fabstore.WebApplication.Controllers;

// Controller for handling review-related actions
public class ReviewController : Controller
    {

    // Logger for logging review events and errors
    private readonly ILogger<ReviewController> _logger;
    // Service for review-related business logic
    private readonly IReviewService _reviewService;

    // Constructor with dependency injection
    public ReviewController(ILogger<ReviewController> logger, IReviewService reviewService)
        {
        _logger = logger;
        _reviewService = reviewService;
        }

    // Not used for routing, but required by MVC conventions
    [NonAction]
    public IActionResult Index()
        {
        return View();
        }

    // Adds a review for a product by the authenticated user
    [HttpPost]
    [Route("api/review/add")]
    [Authorize]
    public async Task<IActionResult> AddReview([FromBody] AddReviewRequest addReviewRequest)
        {
        try
            {
            // Validate the incoming model
            if (!ModelState.IsValid)
                {
                return ResponseFilter.HandleResponse(false, "Invalid Input", HttpStatusCode.BAD_REQUEST);
                }
            // Get the user's identity from claims
            var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdentity))
                {
                return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                }

            // Call the review service to add or update the review
            var serviceResponse = await _reviewService.AddReviewAsync(userIdentity, addReviewRequest.ProductId, addReviewRequest.Rating);
            if (!serviceResponse.Success)
                {
                _logger.LogWarning(serviceResponse.Message);
                return ResponseFilter.HandleResponse(serviceResponse);
                }
            return ResponseFilter.HandleResponse(serviceResponse);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while adding review.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while adding review.", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }
    }

// Model for the add review API request
public class AddReviewRequest
    {
    public int ProductId { get; set; }
    public int Rating { get; set; }
    }