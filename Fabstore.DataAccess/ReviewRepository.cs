using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IReview;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess;

// Repository for review-related database operations
public class ReviewRepository : IReviewRepository
    {

    // Database context for accessing the application's data
    private readonly AppDbContext _context;

    // Constructor that injects the application's database context
    public ReviewRepository(AppDbContext context)
        {
        _context = context;
        }

    // Adds a new review to the database
    public async Task AddReviewAsync(Review review)
        {
        try
            {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            }
        catch (Exception ex)
            {
            // Wrap and throw a custom database exception
            throw new DatabaseException("An error occurred while adding the review.", ex);
            }
        }

    // Retrieves a review for a specific user and product
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

    // Retrieves all reviews for a specific product
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

    // Updates an existing review in the database
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