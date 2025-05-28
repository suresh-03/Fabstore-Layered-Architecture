using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IReview;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Service;

// Service implementation for review-related business logic
public class ReviewService : IReviewService
    {

    // Repository for review data access
    private readonly IReviewRepository _reviewRepository;
    // Factory for creating standardized service responses
    private readonly IServiceResponseFactory _responseFactory;

    // Constructor with dependency injection for repository and response factory
    public ReviewService(IReviewRepository reviewRepository, IServiceResponseFactory responseFactory)
        {
        _reviewRepository = reviewRepository;
        _responseFactory = responseFactory;
        }

    // Adds a new review or updates an existing review for a user and product
    public async Task<IServiceResponse> AddReviewAsync(string userIdentity, int productId, int rating)
        {
        try
            {
            // Validate rating value
            if (rating <= 0)
                {
                return _responseFactory.CreateResponse(false, "Rating must be greater than zero", ActionType.ValidationError);
                }
            int userId = int.Parse(userIdentity);
            // Check if the user has already reviewed this product
            var existingReview = await _reviewRepository.GetReviewByUserIdAsync(userId, productId);
            if (existingReview != null)
                {
                // Update the existing review
                existingReview.UserID = userId;
                existingReview.ProductID = productId;
                existingReview.Rating = rating;
                await _reviewRepository.UpdateReviewAsync(existingReview);
                return _responseFactory.CreateResponse(true, "Review Updated Successfully", ActionType.Updated);
                }
            else
                {
                // Add a new review
                Review review = new Review
                    {
                    UserID = userId,
                    ProductID = productId,
                    Rating = rating
                    };
                await _reviewRepository.AddReviewAsync(review);
                return _responseFactory.CreateResponse(true, "Review Added Successfully", ActionType.Created);
                }
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while adding the review.", ex);
            }
        }

    // Retrieves all reviews for a specific product
    public async Task<IServiceResponse<List<Review>>> GetReviewsAsync(int productId)
        {
        try
            {
            var reviews = await _reviewRepository.GetReviewsAsync(productId);
            return _responseFactory.CreateResponse<List<Review>>(true, "Reviews fetched successfully", ActionType.Retrieved, reviews);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred while fetching the reviews.", ex);
            }
        }

    }