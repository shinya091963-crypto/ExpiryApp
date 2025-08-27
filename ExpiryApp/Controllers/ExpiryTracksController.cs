using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpiryApp.Models;

namespace ExpiryApp.Controllers
{
    public class ExpiryTracksController : Controller
    {
        private readonly AppDbContext _context;

        public ExpiryTracksController(AppDbContext context)
        {
            _context = context;
        }

        // 共通：Device の SelectList を作る
        private async Task<SelectList> BuildDeviceSelectListAsync(int? selected = null)
        {
            var devices = await _context.Devices
                .OrderBy(d => d.Name)
                .Select(d => new
                {
                    d.DeviceId,
                    // 表示を「Name (AssetTag)」にしたい場合：
                    Display = d.Name + (string.IsNullOrEmpty(d.AssetTag) ? "" : $" ({d.AssetTag})")
                })
                .ToListAsync();

            return new SelectList(devices, "DeviceId", "Display", selected);
        }

        // 共通：TrackType の SelectList
        private static SelectList BuildTrackTypeSelectList(string? selected = null)
            => new SelectList(new[] { "Calibration", "Inspection", "Sanitation" }, selected);

        // GET: ExpiryTracks
        public async Task<IActionResult> Index()
        {
            var list = await _context.ExpiryTracks
                .Include(e => e.Device) // 一覧で名前などを出す
                .OrderBy(e => e.ExpiryDate)
                .ToListAsync();

            return View(list);
        }

        // GET: ExpiryTracks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var expiryTrack = await _context.ExpiryTracks
                .Include(e => e.Device)
                .FirstOrDefaultAsync(m => m.ExpiryTrackId == id);

            if (expiryTrack == null) return NotFound();

            return View(expiryTrack);
        }

        // GET: ExpiryTracks/Create
        public async Task<IActionResult> Create()
        {
            ViewData["DeviceId"] = await BuildDeviceSelectListAsync();
            ViewData["TrackType"] = BuildTrackTypeSelectList();
            return View();
        }

        // POST: ExpiryTracks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpiryTrackId,DeviceId,TrackType,ExpiryDate,LeadDays,Note")] ExpiryTrack expiryTrack)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expiryTrack);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // エラー時：プルダウンを復元
            ViewData["DeviceId"] = await BuildDeviceSelectListAsync(expiryTrack.DeviceId);
            ViewData["TrackType"] = BuildTrackTypeSelectList(expiryTrack.TrackType);
            return View(expiryTrack);
        }

        // GET: ExpiryTracks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var expiryTrack = await _context.ExpiryTracks
                .Include(e => e.Device)
                .FirstOrDefaultAsync(e => e.ExpiryTrackId == id);

            if (expiryTrack == null) return NotFound();

            ViewData["DeviceId"] = await BuildDeviceSelectListAsync(expiryTrack.DeviceId);
            ViewData["TrackType"] = BuildTrackTypeSelectList(expiryTrack.TrackType);
            return View(expiryTrack);
        }

        // POST: ExpiryTracks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpiryTrackId,DeviceId,TrackType,ExpiryDate,LeadDays,Note")] ExpiryTrack expiryTrack)
        {
            if (id != expiryTrack.ExpiryTrackId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expiryTrack);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await _context.ExpiryTracks.AnyAsync(e => e.ExpiryTrackId == id);
                    if (!exists) return NotFound();
                    throw;
                }
            }

            // エラー時：プルダウンを復元
            ViewData["DeviceId"] = await BuildDeviceSelectListAsync(expiryTrack.DeviceId);
            ViewData["TrackType"] = BuildTrackTypeSelectList(expiryTrack.TrackType);
            return View(expiryTrack);
        }

        // GET: ExpiryTracks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var expiryTrack = await _context.ExpiryTracks
                .Include(e => e.Device)
                .FirstOrDefaultAsync(m => m.ExpiryTrackId == id);

            if (expiryTrack == null) return NotFound();

            return View(expiryTrack);
        }

        // POST: ExpiryTracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expiryTrack = await _context.ExpiryTracks.FindAsync(id);
            if (expiryTrack != null)
            {
                _context.ExpiryTracks.Remove(expiryTrack);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
