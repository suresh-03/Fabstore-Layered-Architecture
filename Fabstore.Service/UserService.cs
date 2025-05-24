using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Domain.Models;

namespace Fabstore.Service;

public class UserService : IUserService
    {
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
        {
        _repo = repo;
        }



    public async Task<(bool Success, string Message)> SignupAsync(User user)
        {
        var userExists = await _repo.GetUserAsync(user.Email);
        if (userExists != null)
            {
            return (false, "User already exists.");
            }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _repo.AddUserAsync(user);
        return (true, "User Registered Successfully");
        }

    public async Task<(bool Success, string Message, User? User)> SigninAsync(Dictionary<string, string> data)
        {

        // Input validation
        if (data == null ||
            !data.TryGetValue("Email", out var email) ||
            !data.TryGetValue("Password", out var password))
            {
            return (false, "Missing email or password.", null);
            }

        var user = await _repo.GetUserAsync(email);
        if (user == null)
            return (false, "Invalid Email", null);


        bool result = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!result)
            return (false, "Invalid Password", null);

        return (true, "Signed in successfully", user);
        }

    }

