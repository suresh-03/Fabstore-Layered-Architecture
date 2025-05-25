using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IWishlist;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess
    {
    public class WishlistRepository : IWishlistRepository
        {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
            {
            _context = context;
            }
        public async Task AddToWishlistAsync(Wishlist wishlistItem)
            {
            try
                {
                if (wishlistItem.IsDeleted)
                    {
                    wishlistItem.IsDeleted = false;
                    wishlistItem.AddedAt = DateTime.UtcNow;
                    }
                else
                    {
                    await _context.Wishlists.AddAsync(wishlistItem);
                    }
                await _context.SaveChangesAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while adding product to the wishlist", ex);
                }
            }


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
