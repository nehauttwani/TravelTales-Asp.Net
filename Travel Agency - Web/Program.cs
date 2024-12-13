using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data;
using Travel_Agency___Data.ModelManagers;
using Travel_Agency___Data.Models;
using Travel_Agency___Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Service for context objects 
builder.Services.AddDbContext<TravelExpertsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TravelExpertsConnectionString"),
    builder => builder.MigrationsAssembly("Travel Agency - Data")) 
);

// Add identity services
builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
}).AddEntityFrameworkStores<TravelExpertsContext>().AddDefaultTokenProviders();

// Register CustomerManager with DI
builder.Services.AddScoped<CustomerManager>();
builder.Services.AddScoped<AgentsAndAgenciesManager>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<PurchaseService>();
builder.Services.AddScoped<Travel_Agency___Data.Services.EmailService>();

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
