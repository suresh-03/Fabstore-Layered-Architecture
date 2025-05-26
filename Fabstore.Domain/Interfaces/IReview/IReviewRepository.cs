using Fabstore.Domain.Models;

namespace Fabstore.Domain.Interfaces.IReview;

public interface IReviewRepository
    {
    public Task AddReviewAsync(Review review);

    public Task<List<Review>> GetReviewsAsync(int productId);

    public Task UpdateReviewAsync(Review review);

    public Task<Review> GetReviewByUserIdAsync(int userId, int productId);
    }

