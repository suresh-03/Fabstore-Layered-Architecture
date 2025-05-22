using Fabstore.DataAccess.Database;
using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess
    {
    public class UserRepository : IUserRepository
        {

        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
            {
            _context = context;
            }
        public async Task<bool> AddUserAsync(User user)
            {
            try
                {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
                }
            catch (Exception ex)
                {
                throw;
                }
            }

        public async Task<User> GetUserAsync(string email)
            {

            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            }

        public async Task<bool> SaveDbChangesAsync()
            {
            await _context.SaveChangesAsync();
            return true;
            }
        }
    }
