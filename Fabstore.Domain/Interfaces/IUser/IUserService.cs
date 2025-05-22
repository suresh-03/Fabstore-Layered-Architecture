using Fabstore.Domain.Models;

namespace Fabstore.Domain.Interfaces.IUser;

public interface IUserService
    {
    public Task<(bool Success, string Message)> SignupAsync(User user);

    public Task<(bool Success, string Message, User? User)> SigninAsync(Dictionary<string, string> data);
    }

