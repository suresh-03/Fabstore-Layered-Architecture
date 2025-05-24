using Fabstore.Domain.Models;

namespace Fabstore.Domain.Interfaces.IUser
    {


    public interface IUserRepository : IBaseRepository
        {
        public Task<bool> AddUserAsync(User user);

        public Task<User> GetUserAsync(string email);
        }

    }

