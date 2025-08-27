using Microsoft.EntityFrameworkCore;
using ExpiryApp.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC (Controllers with Views) ��L����
builder.Services.AddControllersWithViews();

// DbContext ��o�^�iappsettings.json �� "ConnectionStrings:Default" ���g�p�j
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// �G���[�n���h�����O
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// �R���g���[���[�̊��胋�[�g�ݒ�
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
