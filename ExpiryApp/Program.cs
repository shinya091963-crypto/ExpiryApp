using Microsoft.EntityFrameworkCore;
using ExpiryApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ÅöîCà”ÅFDevices Çñæé¶
app.MapControllerRoute(
    name: "devices",
    pattern: "devices/{action=Index}/{id?}",
    defaults: new { controller = "Devices" });

app.Run();
