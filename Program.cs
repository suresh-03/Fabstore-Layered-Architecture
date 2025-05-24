using Fabstore.DataAccess;
using Fabstore.DataAccess.Database;
using Fabstore.Domain.Interfaces.ICart;
using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Service;
using FabstoreWebApplication.Configs;
using FabstoreWebApplication.Filters;
using FabstoreWebApplication.Mapping;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

// Service to Filter ObjectResult to JsonResult
builder.Services.AddScoped<ApiResponseFilter>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});

// Service for Cookie based Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/SignIn";
        options.LogoutPath = "/Auth/Signout";
        options.AccessDeniedPath = "/Error";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// Mapper for ViewModel to EFModel and Vice-Versa
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Adding Repository Dependencies
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Adding Service Dependencies
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IUserService, UserService>();


// For Session Handling
builder.Services.AddSession();



// Service for DBContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Service for ServerConfig
builder.Services.Configure<ServerConfig>(builder.Configuration.GetSection("ServerConfig"));

var app = builder.Build();

// redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    }


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
