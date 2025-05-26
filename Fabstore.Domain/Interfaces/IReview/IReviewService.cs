using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Domain.Interfaces.IReview;

public interface IReviewService
    {
    public Task<IServiceResponse> AddReviewAsync(string userIdentity, int productId, int rating);

    public Task<IServiceResponse<List<Review>>> GetReviewsAsync(int productId);

    }

