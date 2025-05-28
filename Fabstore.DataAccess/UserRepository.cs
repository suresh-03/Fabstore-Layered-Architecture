using Fabstore.DataAccess.Database;
using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabstore.DataAccess
    {
    // Repository for user-related database operations
    public class UserRepository : IUserRepository
        {

        // Database context for accessing user data
        private readonly AppDbContext _context;

        // Constructor with dependency injection for the database context
        public UserRepository(AppDbContext context)
            {
            _context = context;
            }

        // Adds a new user to the database
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
                // Wrap and throw a custom database exception
                throw new DatabaseException("An error occurred while adding the user.", ex);
                }
            }

        // Retrieves a user by email address
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
