using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IWishlist;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess
    {
    // Repository for wishlist-related database operations
    public class WishlistRepository : IWishlistRepository
        {
        // Database context for accessing wishlist data
        private readonly AppDbContext _context;

        // Constructor with dependency injection for the database context
        public WishlistRepository(AppDbContext context)
            {
            _context = context;
            }

        // Adds an item to the wishlist or restores a previously deleted item
        public async Task AddToWishlistAsync(Wishlist wishlistItem)
            {
            try
                {
                if (wishlistItem.IsDeleted)
                    {
                    // Restore the wishlist item if it was previously marked as deleted
                    wishlistItem.IsDeleted = false;
                    wishlistItem.AddedAt = DateTime.UtcNow;
                    }
                else
                    {
                    // Add new wishlist item
                    await _context.Wishlists.AddAsync(wishlistItem);
                    }
                await _context.SaveChangesAsync();
                }
            catch (Exception ex)
                {
                // Wrap and throw a custom database exception
                throw new DatabaseException("Error occurred while adding product to the wishlist", ex);
                }
            }

        // Retrieves a specific wishlist item for a user and product variant
        public async Task<Wishlist> GetWishlistItemAsync(int userId, int productVariantId)
            {
            try
                {
                return await _context.Wishlists
                        .Where(item => item.VariantID == productVariantId && item.UserID == userId)
                        .FirstOrDefaultAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while retrieving cart item", ex);
                }
            }

        // Retrieves all active wishlist items for a user, including related product and image data
        public async Task<List<Wishlist>> GetWishlistItemsAsync(int userId)
            {
            try
                {
                return await _context.Wishlists
                    .Include(item => item.Variant)
                        .ThenInclude(variant => variant.Product)
                        .ThenInclude(category => category.Category)
                    .Include(item => item.Variant)
                        .ThenInclude(variant => variant.Images)
                    .Where(item => item.UserID == userId && !item.IsDeleted)
                    .ToListAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while retrieving wishlist items", ex);
                }
            }

        // Marks a wishlist item as deleted
        public async Task RemoveFromWishlistAsync(Wishlist wishlistItem)
            {
            try
                {
                wishlistItem.IsDeleted = true;
                wishlistItem.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while removing the product from the wishlist", ex);
                }
            }
        }
    }