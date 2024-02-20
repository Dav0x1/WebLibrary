using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebLibrary.Controllers;
using WebLibrary.Models;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and views services
builder.Services.AddControllersWithViews();

// Add Entity Framework DbContext with SQL Server as the database provider
builder.Services.AddDbContext<WLDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure authentication with cookie authentication
builder.Services.AddAuthentication(o =>
{
    // Set default authentication schemes
    o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
{
    // Configure cookie properties
    o.Cookie.IsEssential = true;
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Strict;
    o.Cookie.SecurePolicy = CookieSecurePolicy.None;
    o.Cookie.MaxAge = TimeSpan.FromMinutes(5);
    o.AccessDeniedPath = "/Home";
    o.LoginPath = "/Login";
    o.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
});

// Add email sender as a transient service
builder.Services.AddTransient<EmailSender>();

// Configure session
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Set session idle timeout
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    // Use exception handler for non-development environments
    app.UseExceptionHandler("/Home/Error");

    // Enable HTTP Strict Transport Security (HSTS)
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Use session
app.UseSession();

// Run the application
app.Run();