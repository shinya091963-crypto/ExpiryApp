using Microsoft.EntityFrameworkCore;
using ExpiryApp.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC (Controllers with Views) を有効化
builder.Services.AddControllersWithViews();

// DbContext を登録（appsettings.json の "ConnectionStrings:Default" を使用）
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// エラーハンドリング
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// コントローラーの既定ルート設定
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
