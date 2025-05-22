using AutoMapper;
using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Domain.Models;
using FabstoreWebApplication.Filters;
using FabstoreWebApplication.Helpers;
using FabstoreWebApplication.ViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FabstoreWebApplication.Controllers;

[ServiceFilter(typeof(ApiResponseFilter))]
public class AuthController : Controller
    {

    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;
    private readonly IAntiforgery _antiforgery;
    private readonly IMapper _mapper;
    public AuthController(ILogger<AuthController> logger, IAntiforgery antiforgery, IMapper mapper, IUserService userService)
        {

        _logger = logger;
        _antiforgery = antiforgery;
        _userService = userService;
        _mapper = mapper;
        }
    public IActionResult Index()
        {
        return RedirectToAction("SignUp");
        }
    [HttpGet]
    public IActionResult SignUp()
        {
        return View();
        }


    [HttpPost]
    public async Task<IActionResult> SignUpAPI([FromBody] UserView model)
        {
        try
            {
            await _antiforgery.ValidateRequestAsync(HttpContext);
            try
                {
                if (!ModelState.IsValid)
                    {
                    return ObjectResultHelper.CreateObjectResult("error", "Invalid input data.", 400);
                    }

                var userModel = _mapper.Map<User>(model);

                var result = await _userService.SignupAsync(userModel);

                if (!result.Success)
                    {
                    return ObjectResultHelper.CreateObjectResult("error", result.Message, 400);
                    }

                _logger.LogInformation("User registered successfully");
                return ObjectResultHelper.CreateObjectResult("success", result.Message, 201, Url.Action("SignIn", "Auth") ?? "");


                }
            catch (DbUpdateException dbEx)
                {
                _logger.LogError(dbEx, "Database error occurred during signup.");
                return ObjectResultHelper.CreateObjectResult("error", "Database error occurred during signup. Please try again later.", 500);
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "An unexpected error occurred during signup.");
                return ObjectResultHelper.CreateObjectResult("error", "An unexpected error occurred. Please try again later.", 500);

                }
            }
        catch (AntiforgeryValidationException ex)
            {
            _logger.LogError(ex, "Invalid anti-forgery token.");
            return ObjectResultHelper.CreateObjectResult("error", "Invalid anti-forgery token.", 403);

            }
        }

    [HttpGet]
    public IActionResult SignIn()
        {
        return View();
        }

    [HttpPost]
    public async Task<IActionResult> SignInAPI([FromBody] Dictionary<string, string> bodyData)
        {
        try
            {
            // Validate anti-forgery token (for [FromBody] manually)
            await _antiforgery.ValidateRequestAsync(HttpContext);

            var result = await _userService.SigninAsync(bodyData);

            if (!result.Success)
                {
                return ObjectResultHelper.CreateObjectResult("error", result.Message, 400);

                }

            var user = result.User;

            // Setup claims
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

            // Sign in user
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return ObjectResultHelper.CreateObjectResult("success", "User Signed in Successfully", 200, Url.Action("Index", "Home") ?? "");
            }
        catch (AntiforgeryValidationException ex)
            {
            _logger.LogWarning(ex, "Antiforgery validation failed.");
            return ObjectResultHelper.CreateObjectResult("error", "Invalid request (possible CSRF attempt).", 403);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Unexpected error during signin.");
            return ObjectResultHelper.CreateObjectResult("error", "An unexpected error occurred. Please try again later.", 500);
            }
        }


    [HttpGet]
    public async Task<IActionResult> Signout()
        {
        try
            {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Unexpected error during signout.");
            return ObjectResultHelper.CreateObjectResult("error", "An unexpected error occurred. Please try again later.", 500);
            }

        return RedirectToAction("SignIn", "Auth");
        }


    }


