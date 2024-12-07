using Microsoft.EntityFrameworkCore;
using Travel_Agency___Data;
using Travel_Agency___Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Service for context objects 
builder.Services.AddDbContext<TravelExpertsContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TravelExpertsContext")));

builder.Services.AddScoped<WalletService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
