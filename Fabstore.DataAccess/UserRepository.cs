using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
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
                throw new DatabaseException("An error occurred while adding the user.", ex);
                }
            }

        public async Task<User> GetUserAsync(string email)
            {
            try
                {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                }
            catch (Exception ex)
                {
                throw new DatabaseException("An error occurred while retrieving the user.", ex);
                }
            }


        }
    }
