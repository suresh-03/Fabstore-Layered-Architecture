using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Domain.Interfaces.IUser;

public interface IUserService
    {
    public Task<IServiceResponse> SignupAsync(User user);

    public Task<IServiceResponse<User>> SigninAsync(Dictionary<string, string> data);
    }

