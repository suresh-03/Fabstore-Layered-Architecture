using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;
using Fabstore.Service.ResponseFormat;

namespace Fabstore.Service;

public class UserService : IUserService
    {
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
        {
        _repo = repo;
        }



    public async Task<IServiceResponse> SignupAsync(User user)
        {
        try
            {
            var userExists = await _repo.GetUserAsync(user.Email);
            if (userExists != null)
                {
                return new ServiceResponse(false, "User Already Exists", ActionType.Conflict);
                }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _repo.AddUserAsync(user);
            return new ServiceResponse(true, "User Registered Successfully", ActionType.Created);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred during signup.", ex);
            }
        }

    public async Task<IServiceResponse<User>> SigninAsync(Dictionary<string, string> data)
        {
        try
            {
            if (data == null ||
                !data.TryGetValue("Email", out var email) ||
                !data.TryGetValue("Password", out var password))
                {
                return new ServiceResponse<User>(false, "Missing email or password.", ActionType.ValidationError, null);
                }

            var user = await _repo.GetUserAsync(email);
            if (user == null)
                {
                return new ServiceResponse<User>(false, "Invalid Email", ActionType.ValidationError, null);
                }

            bool result = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!result)
                {
                return new ServiceResponse<User>(false, "Invalid Password", ActionType.ValidationError, null);
                }

            return new ServiceResponse<User>(true, "User Signed in Successfully", ActionType.Retrieved, user);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred during signin.", ex);
            }
        }


    }

