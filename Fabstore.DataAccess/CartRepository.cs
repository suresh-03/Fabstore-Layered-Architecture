using Fabstore.DataAccess.Database;
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
                await _context.Carts.AddAsync(cartItem);
                await _context.SaveChangesAsync();
                return true;
                }
            catch (Exception ex)
                {
                throw;
                }
            }

        public async Task<int> GetCartCountAsync(int userId)
            {
            return await _context.Carts
                .Where(cartItem => cartItem.UserID == userId && !cartItem.IsDeleted)
                .CountAsync();
            }

        public async Task<Cart> GetCartItemAsync(int userId, int productVariantId)
            {
            return await _context.Carts
                    .Where(cart => cart.VariantID == productVariantId && cart.UserID == userId)
                    .FirstOrDefaultAsync();
            }

        public async Task<List<Cart>> GetCartItemsAsync(int userId)
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

        public async Task<bool> RemoveFromCartAsync()
            {
            try
                {
                await _context.SaveChangesAsync();
                return true;
                }
            catch (Exception ex)

                {
                throw;
                }
            }

        public async Task<bool> SaveDbChangesAsync()
            {
            await _context.SaveChangesAsync();
            return true;
            }
        }
    }
