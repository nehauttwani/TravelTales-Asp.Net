using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register services directly
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<PurchaseService>();

// Register DbContext
builder.Services.AddDbContext<TravelExpertsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TravelExpertsConnectionString")));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<TravelExpertsContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
