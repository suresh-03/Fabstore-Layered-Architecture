using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Domain.Models;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FabstoreWebApplication.Controllers;

// Controller for handling authentication-related actions
public class AuthController : Controller
    {

    // Service for user-related business logic
    private readonly IUserService _userService;
    // Logger for logging authentication events and errors
    private readonly ILogger<AuthController> _logger;
    // Service for anti-forgery token validation
    private readonly IAntiforgery _antiforgery;

    // Constructor with dependency injection
    public AuthController(ILogger<AuthController> logger, IAntiforgery antiforgery, IUserService userService)
        {
        _logger = logger;
        _antiforgery = antiforgery;
        _userService = userService;
        }

    // Redirects to the SignUp page
    public IActionResult Index()
        {
        return RedirectToAction("SignUp");
        }

    // Displays the SignUp view
    [HttpGet]
    public IActionResult SignUp()
        {
        return View();
        }

    // Handles user registration via API
    [HttpPost]
    public async Task<IActionResult> SignUpAPI([FromBody] User model)
        {
        try
            {
            // Validate anti-forgery token
            await _antiforgery.ValidateRequestAsync(HttpContext);
            try
                {
                // Validate model state
                if (!ModelState.IsValid)
                    {
                    return ResponseFilter.HandleResponse(false, "Invalid Input", HttpStatusCode.BAD_REQUEST);
                    }

                var serviceResponse = await _userService.SignupAsync(model);

                if (!serviceResponse.Success)
                    {
                    _logger.LogWarning(serviceResponse.Message);
                    return ResponseFilter.HandleResponse(serviceResponse);
                    }

                // Registration successful, redirect to SignIn
                return ResponseFilter.HandleResponse(true, "User registered successfully.", HttpStatusCode.CREATED, Url.Action("SignIn", "Auth"));
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "An unexpected error occurred during signup.");
                return ResponseFilter.HandleResponse(false, "Something went wrong. Please try again later.", HttpStatusCode.INTERNAL_SERVER_ERROR);
                }
            }
        catch (AntiforgeryValidationException ex)
            {
            _logger.LogError(ex, "Invalid anti-forgery token.");
            return ResponseFilter.HandleResponse(false, "Something went wrong. Please try again later.", HttpStatusCode.FORBIDDEN);
            }
        }

    // Displays the SignIn view
    [HttpGet]
    public IActionResult SignIn()
        {
        return View();
        }

    // Handles user sign-in via API
    [HttpPost]
    public async Task<IActionResult> SignInAPI([FromBody] Dictionary<string, string> bodyData)
        {
        try
            {
            // Validate anti-forgery token (for [FromBody] manually)
            await _antiforgery.ValidateRequestAsync(HttpContext);

            var serviceResponse = await _userService.SigninAsync(bodyData);

            if (!serviceResponse.Success)
                {
                _logger.LogWarning(serviceResponse.Message);
                return ResponseFilter.HandleResponse(serviceResponse);
                }

            var user = serviceResponse.Data;

            // Setup claims for authentication
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
                {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

            // Sign in user with cookie authentication
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return ResponseFilter.HandleResponse(true, "User signedin successfully.", HttpStatusCode.OK, Url.Action("Index", "Home"));
            }
        catch (AntiforgeryValidationException ex)
            {
            _logger.LogWarning(ex, "Antiforgery validation failed.");
            return ResponseFilter.HandleResponse(false, "Invalid Request", HttpStatusCode.FORBIDDEN);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Unexpected error during signin.");
            return ResponseFilter.HandleResponse(false, "Something went wrong. Please try again later.", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }

    // Handles user signout and redirects to SignIn
    [HttpGet]
    public async Task<IActionResult> Signout()
        {
        try
            {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn", "Auth");
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Unexpected error during signout.");
            return ResponseFilter.HandleResponse(false, "Something went wrong. Please try again later.", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }
    }