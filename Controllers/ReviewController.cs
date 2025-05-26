using Fabstore.Domain.Interfaces.IReview;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fabstore.WebApplication.Controllers;

public class ReviewController : Controller
    {

    private readonly ILogger<ReviewController> _logger;
    private readonly IReviewService _reviewService;

    public ReviewController(ILogger<ReviewController> logger, IReviewService reviewService)
        {
        _logger = logger;
        _reviewService = reviewService;
        }

    [NonAction]
    public IActionResult Index()
        {
        return View();
        }

    [HttpPost]
    [Route("api/review/add")]
    [Authorize]
    public async Task<IActionResult> AddReview([FromBody] AddReviewRequest addReviewRequest)
        {
        try
            {
            if (!ModelState.IsValid)
                {
                return ResponseFilter.HandleResponse(false, "Invalid Input", HttpStatusCode.BAD_REQUEST);
                }
            var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdentity))
                {
                return ResponseFilter.HandleResponse(false, "User is not Authenticated", HttpStatusCode.UNAUTHORIZED);
                }

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

public class AddReviewRequest
    {
    public int ProductId { get; set; }
    public int Rating { get; set; }
    }


