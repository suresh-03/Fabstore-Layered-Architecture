using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IReview;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Service;

public class ReviewService : IReviewService
    {

    private readonly IReviewRepository _reviewRepository;
    private readonly IServiceResponseFactory _responseFactory;

    public ReviewService(IReviewRepository reviewRepository, IServiceResponseFactory responseFactory)
        {
        _reviewRepository = reviewRepository;
        _responseFactory = responseFactory;
        }
    public async Task<IServiceResponse> AddReviewAsync(string userIdentity, int productId, int rating)
        {
        try
            {
            if (rating <= 0)
                {
                return _responseFactory.CreateResponse(false, "Rating must be greater than zero", ActionType.ValidationError);
                }
            int userId = int.Parse(userIdentity);
            var existingReview = await _reviewRepository.GetReviewByUserIdAsync(userId, productId);
            if (existingReview != null)
                {
                existingReview.UserID = userId;
                existingReview.ProductID = productId;
                existingReview.Rating = rating;
                await _reviewRepository.UpdateReviewAsync(existingReview);
                return _responseFactory.CreateResponse(true, "Review Updated Successfully", ActionType.Updated);
                }
            else
                {
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

