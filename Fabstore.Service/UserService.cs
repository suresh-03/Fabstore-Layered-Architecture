using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;

namespace Fabstore.Service;

// Service implementation for user-related business logic
public class UserService : IUserService
    {
    // Repository for user data access
    private readonly IUserRepository _userRepository;
    // Factory for creating standardized service responses
    private readonly IServiceResponseFactory _responseFactory;

    // Constructor with dependency injection for repository and response factory
    public UserService(IUserRepository userRepository, IServiceResponseFactory responseFactory)
        {
        _userRepository = userRepository;
        _responseFactory = responseFactory;
        }

    // Handles user signup logic
    public async Task<IServiceResponse> SignupAsync(User user)
        {
        try
            {
            // Check if a user with the same email already exists
            var userExists = await _userRepository.GetUserAsync(user.Email);
            if (userExists != null)
                {
                return _responseFactory.CreateResponse(false, "User Already Exists", ActionType.Conflict);
                }

            // Hash the user's password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            // Add the new user to the database
            await _userRepository.AddUserAsync(user);
            return _responseFactory.CreateResponse(true, "User Registered Successfully", ActionType.Created);
            }
        catch (Exception ex)
            {
            // Wrap and throw a custom service exception
            throw new ServiceException("An unexpected error occurred during signup.", ex);
            }
        }

    // Handles user signin logic
    public async Task<IServiceResponse<User>> SigninAsync(Dictionary<string, string> data)
        {
        try
            {
            // Validate input data for email and password
            if (data == null ||
                !data.TryGetValue("Email", out var email) ||
                !data.TryGetValue("Password", out var password))
                {
                return _responseFactory.CreateResponse<User>(false, "Missing email or password.", ActionType.ValidationError, null);
                }

            // Retrieve user by email
            var user = await _userRepository.GetUserAsync(email);
            if (user == null)
                {
                return _responseFactory.CreateResponse<User>(false, "Invalid Email", ActionType.ValidationError, null);
                }

            // Verify the provided password against the stored hash
            bool result = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!result)
                {
                return _responseFactory.CreateResponse<User>(false, "Invalid Password", ActionType.ValidationError, null);
                }

            // Return success response with user data
            return _responseFactory.CreateResponse<User>(true, "User Signed in Successfully", ActionType.Retrieved, user);
            }
        catch (Exception ex)
            {
            throw new ServiceException("An unexpected error occurred during signin.", ex);
            }
        }
    }