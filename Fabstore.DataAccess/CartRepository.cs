using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.ICart;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess
    {
    // Repository implementation for cart-related database operations
    public class CartRepository : ICartRepository
        {

        // Database context for accessing the application's data
        private readonly AppDbContext _context;

        // Constructor that injects the application's database context
        public CartRepository(AppDbContext context)
            {
            _context = context;
            }

        // Adds a cart item to the database or restores a previously deleted item
        public async Task<bool> AddToCartAsync(Cart cartItem)
            {
            try
                {
                if (cartItem.IsDeleted)
                    {
                    // Restore the cart item if it was previously marked as deleted
                    cartItem.IsDeleted = false;
                    cartItem.AddedAt = DateTime.UtcNow;
                    }
                else
                    {
                    // Add new cart item
                    await _context.Carts.AddAsync(cartItem);
                    }
                await _context.SaveChangesAsync();
                return true;
                }
            catch (Exception ex)
                {
                // Wrap and throw a custom database exception
                throw new DatabaseException("Error occurred while adding product to the cart", ex);
                }
            }

        // Returns the number of active (not deleted) cart items for a user
        public async Task<int> GetCartCountAsync(int userId)
            {
            try
                {
                return await _context.Carts
                    .Where(cartItem => cartItem.UserID == userId && !cartItem.IsDeleted)
                    .CountAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while retrieving cart count", ex);
                }
            }

        // Retrieves a specific cart item for a user and product variant
        public async Task<Cart> GetCartItemAsync(int userId, int productVariantId)
            {
            try
                {
                return await _context.Carts
                        .Where(cart => cart.VariantID == productVariantId && cart.UserID == userId)
                        .FirstOrDefaultAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while retrieving cart item", ex);
                }
            }

        // Retrieves all active cart items for a user, including related product and image data
        public async Task<List<Cart>> GetCartItemsAsync(int userId)
            {
            try
                {
                return await _context.Carts
                    .Include(cart => cart.Variant)
                        .ThenInclude(variant => variant.Product)
                        .ThenInclude(category => category.Category)
                    .Include(cart => cart.Variant)
                        .ThenInclude(variant => variant.Images)
                    .Where(cart => cart.UserID == userId && !cart.IsDeleted)
                    .ToListAsync();
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while retrieving cart items", ex);
                }
            }

        // Marks a cart item as deleted and resets its quantity
        public async Task<bool> RemoveFromCartAsync(Cart cartItem)
            {
            try
                {
                cartItem.IsDeleted = true;
                cartItem.DeletedAt = DateTime.UtcNow;
                cartItem.Quantity = 1; // Reset quantity to default
                await _context.SaveChangesAsync();
                return true;
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while removing the product from the cart", ex);
                }
            }

        // Updates the quantity of a specific cart item for a user
        public async Task UpdateCartQuantity(int userId, int cartId, int quantity)
            {
            try
                {
                await _context.Carts.Where(cart => cart.UserID == userId && cart.CartID == cartId)
                    .ExecuteUpdateAsync(cart => cart
                    .SetProperty(c => c.Quantity, quantity));
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while updating cart quantity", ex);
                }
            }
        }
    }