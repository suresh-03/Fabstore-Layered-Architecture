using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Service;

public class UserService : IUserService
    {
    private readonly IUserRepository _userRepository;
    private readonly IServiceResponseFactory _responseFactory;

    public UserService(IUserRepository userRepository, IServiceResponseFactory responseFactory)
        {
        _userRepository = userRepository;
        _responseFactory = responseFactory;
        }



    public async Task<IServiceResponse> SignupAsync(User user)
        {
        try
            {
            var userExists = await _userRepository.GetUserAsync(user.Email);
            if (userExists != null)
                {
                return _responseFactory.CreateResponse(false, "User Already Exists", ActionType.Conflict);
                }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.AddUserAsync(user);
            return _responseFactory.CreateResponse(true, "User Registered Successfully", ActionType.Created);
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
                return _responseFactory.CreateResponse<User>(false, "Missing email or password.", ActionType.ValidationError, null);
                }

            var user = await _userRepository.GetUserAsync(email);
            if (user == null)
                {
                return _responseFactory.CreateResponse<User>(false, "Invalid Email", ActionType.ValidationError, null);
                }

            bool result = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!result)
                {
                return _responseFactory.CreateResponse<User>(false, "Invalid Password", ActionType.ValidationError, null);
                }

            return _responseFactory.CreateResponse<User>(true, "User Signed in Successfully", ActionType.Retrieved, user);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred during signin.", ex);
            }
        }


    }

