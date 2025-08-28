using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpiryApp.Models;

namespace ExpiryApp.Controllers
{
    public class DevicesController : Controller
    {
        private readonly AppDbContext _context;
        public DevicesController(AppDbContext context) => _context = context;

        // ====== �ꗗ ======
        // GET: /Devices?q=...
        [HttpGet]
        public async Task<IActionResult> Index(string? q)
        {
            IQueryable<Device> query = _context.Devices.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(d =>
                    (d.AssetTag != null && EF.Functions.Like(d.AssetTag, $"%{q}%")) ||
                    (d.Name != null && EF.Functions.Like(d.Name, $"%{q}%")) ||
                    (d.Location != null && EF.Functions.Like(d.Location, $"%{q}%")) ||
                    (d.OwnerDept != null && EF.Functions.Like(d.OwnerDept, $"%{q}%"))
                );
            }

            ViewData["Title"] = "�@��ꗗ";
            ViewBag.Query = q;
            ViewBag.TotalCount = await query.CountAsync();

            var list = await query
                .OrderBy(d => d.Name)
                .ThenBy(d => d.AssetTag)
                .ToListAsync();

            return View(list); // Views/Devices/Index.cshtml ��Ԃ�
        }

        // ====== �ڍ� ======
        // GET: /Devices/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var device = await _context.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DeviceId == id);

            if (device == null) return NotFound();
            return View(device); // Views/Devices/Details.cshtml
        }

        // ====== �V�K�쐬 ======
        // GET: /Devices/Create
        [HttpGet]
        public IActionResult Create() => View(); // Views/Devices/Create.cshtml

        // POST: /Devices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeviceId,AssetTag,Name,Location,OwnerDept")] Device device)
        {
            // �����͂� NULL �Ɋ񂹂�iDB�� NULL�j
            if (string.IsNullOrWhiteSpace(device.AssetTag))
                device.AssetTag = null;

            // �d���`�F�b�N�iNULL�̓X�L�b�v�j
            if (device.AssetTag != null)
            {
                var duplicate = await _context.Devices
                    .AsNoTracking()
                    .AnyAsync(d => d.AssetTag == device.AssetTag);

                if (duplicate)
                    ModelState.AddModelError(nameof(device.AssetTag), "����ID�͊��Ɏg�p����Ă��܂��B");
            }

            if (!ModelState.IsValid) return View(device);

            _context.Add(device);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = "�@���o�^���܂����B";
            return RedirectToAction(nameof(Index));
        }

        // ====== �ҏW ======
        // GET: /Devices/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var device = await _context.Devices.FindAsync(id);
            if (device == null) return NotFound();

            return View(device); // Views/Devices/Edit.cshtml
        }

        // POST: /Devices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DeviceId,AssetTag,Name,Location,OwnerDept")] Device device)
        {
            if (id != device.DeviceId) return NotFound();

            if (string.IsNullOrWhiteSpace(device.AssetTag))
                device.AssetTag = null;

            // �����ȊO�œ���ID��������
            if (device.AssetTag != null)
            {
                var duplicate = await _context.Devices
                    .AsNoTracking()
                    .AnyAsync(d => d.AssetTag == device.AssetTag && d.DeviceId != device.DeviceId);

                if (duplicate)
                    ModelState.AddModelError(nameof(device.AssetTag), "����ID�͊��Ɏg�p����Ă��܂��B");
            }

            if (!ModelState.IsValid) return View(device);

            try
            {
                _context.Update(device);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "�@������X�V���܂����B";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Devices.AnyAsync(e => e.DeviceId == id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // ====== �폜 ======
        // GET: /Devices/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var device = await _context.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DeviceId == id);

            if (device == null) return NotFound();
            return View(device); // Views/Devices/Delete.cshtml
        }

        // POST: /Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "�@����폜���܂����B";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
