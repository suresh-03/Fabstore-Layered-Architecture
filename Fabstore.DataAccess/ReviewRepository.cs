using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IReview;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess;

public class ReviewRepository : IReviewRepository
    {

    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
        {
        _context = context;
        }
    public async Task AddReviewAsync(Review review)
        {
        try
            {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            }
        catch (Exception ex)
            {
            throw new DatabaseException("An error occurred while adding the review.", ex);
            }
        }

    public async Task<Review> GetReviewByUserIdAsync(int userId, int productId)
        {
        try
            {
            return await _context.Reviews
                .Where(review => review.UserID == userId && review.ProductID == productId)
                .FirstOrDefaultAsync();

            }
        catch (Exception ex)
            {
            throw new DatabaseException("An error occurred while fetching the review.", ex);
            }
        }

    public async Task<List<Review>> GetReviewsAsync(int productId)
        {
        try
            {
            return await _context.Reviews
                .Where(review => review.ProductID == productId)
                .ToListAsync();
            }
        catch (Exception ex)
            {
            throw new DatabaseException("An error occurred while fetching the reviews.", ex);
            }
        }

    public async Task UpdateReviewAsync(Review review)
        {
        try
            {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            }
        catch (Exception ex)
            {
            throw new DatabaseException("An error occurred while updating the review.", ex);
            }
        }

    }

