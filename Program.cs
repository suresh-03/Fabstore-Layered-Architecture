using Fabstore.DataAccess;
using Fabstore.DataAccess.Database;
using Fabstore.Domain.Interfaces.ICart;
using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.Domain.Interfaces.IUser;
using Fabstore.Domain.Interfaces.IWishlist;
using Fabstore.Domain.ResponseFormat;
using Fabstore.Service;
using Fabstore.Service.ResponseFormat;
using FabstoreWebApplication.Configs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.



builder.Services.AddControllersWithViews();

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

// Adding Repository Dependencies
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();


// Adding Service Dependencies
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();


// For Common Service Response
builder.Services.AddScoped<IServiceResponseFactory, ServiceResponseFactory>();


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
