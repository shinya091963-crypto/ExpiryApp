using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpiryApp.Models;
using ExpiryApp.Models.ViewModels;

namespace ExpiryApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var within7 = today.AddDays(7);
            var within30 = today.AddDays(30);

            var vm = new DashboardViewModel
            {
                OverdueCount = await _db.ExpiryTracks.CountAsync(x => x.ExpiryDate < today),
                Within7DaysCount = await _db.ExpiryTracks.CountAsync(x => x.ExpiryDate >= today && x.ExpiryDate <= within7),
                Within30DaysCount = await _db.ExpiryTracks.CountAsync(x => x.ExpiryDate >= today && x.ExpiryDate <= within30)
            };
            return View(vm);
        }
    }
}
