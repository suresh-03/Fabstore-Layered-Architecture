using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.ICart;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess
    {
    public class CartRepository : ICartRepository
        {

        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
            {
            _context = context;
            }

        public async Task<bool> AddToCartAsync(Cart cartItem)
            {
            try
                {
                if (cartItem.IsDeleted)
                    {
                    cartItem.IsDeleted = false;
                    }
                else
                    {
                    await _context.Carts.AddAsync(cartItem);
                    }
                await _context.SaveChangesAsync();
                return true;
                }
            catch (Exception ex)
                {
                throw new DatabaseException("Error occurred while adding product to the cart", ex);
                }
            }

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

        public async Task<bool> RemoveFromCartAsync(Cart cartItem)
            {
            try
                {
                cartItem.IsDeleted = true;
                cartItem.DeletedAt = DateTime.UtcNow;
                cartItem.Quantity = 1;
                await _context.SaveChangesAsync();
                return true;
                }
            catch (Exception ex)

                {
                throw new DatabaseException("Error occurred while removing the product from the cart", ex);
                }
            }



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
