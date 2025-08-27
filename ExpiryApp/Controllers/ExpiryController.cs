using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpiryApp.Models;

namespace ExpiryApp.Controllers
{
    public class ExpiryController : Controller
    {
        private readonly AppDbContext _db;

        public ExpiryController(AppDbContext db)
        {
            _db = db;
        }

        // /Expiry?withinDays=30&dept=内科&sort=date_asc&page=1&pageSize=12
        public async Task<IActionResult> Index(int withinDays = 30, string? dept = null, string? sort = "date_asc", int page = 1, int pageSize = 12)
        {
            var today = DateTime.Today;
            var until = today.AddDays(withinDays);

            var query = _db.ExpiryTracks
                .Include(x => x.Device)
                .Where(x => x.ExpiryDate <= until);

            if (!string.IsNullOrWhiteSpace(dept))
                query = query.Where(x => x.Device != null && x.Device.OwnerDept == dept);

            // 並び替え
            query = sort switch
            {
                "date_desc" => query.OrderByDescending(x => x.ExpiryDate),
                "type_asc" => query.OrderBy(x => x.TrackType).ThenBy(x => x.ExpiryDate),
                "type_desc" => query.OrderByDescending(x => x.TrackType).ThenBy(x => x.ExpiryDate),
                "device_asc" => query.OrderBy(x => x.Device!.Name).ThenBy(x => x.ExpiryDate),
                "device_desc" => query.OrderByDescending(x => x.Device!.Name).ThenBy(x => x.ExpiryDate),
                _ => query.OrderBy(x => x.ExpiryDate) // 既定: 期限日昇順
            };

            // ページング
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 12;
            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.WithinDays = withinDays;
            ViewBag.Today = today;
            ViewBag.Dept = dept;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Total = total;

            return View(items);
        }


        // CSV ダウンロード: /Expiry/ExportCsv?withinDays=60&dept=内科
        [HttpGet]
        public async Task<IActionResult> ExportCsv(int withinDays = 30, string? dept = null)
        {
            var today = DateTime.Today;
            var until = today.AddDays(withinDays);

            var query = _db.ExpiryTracks
                .Include(x => x.Device)
                .Where(x => x.ExpiryDate <= until);

            if (!string.IsNullOrWhiteSpace(dept))
                query = query.Where(x => x.Device != null && x.Device.OwnerDept == dept);

            var list = await query
                .OrderBy(x => x.ExpiryDate)
                .Select(x => new
                {
                    Date = x.ExpiryDate.ToString("yyyy/MM/dd"),
                    TrackType = x.TrackType,
                    DeviceName = x.Device != null ? x.Device.Name : null,
                    AssetTag = x.Device != null ? x.Device.AssetTag : null,
                    Location = x.Device != null ? x.Device.Location : null,
                    Dept = x.Device != null ? x.Device.OwnerDept : null,
                    Note = x.Note
                })
                .ToListAsync();

            // null 安全なエスケープ関数
            string Esc(string? s) => $"\"{(s ?? string.Empty).Replace("\"", "\"\"")}\"";

            var sb = new StringBuilder();

            // Excel が区切りを正しく解釈できるようにヒント行を追加
            sb.AppendLine("sep=,");
            sb.AppendLine("期限日,種類,機器名,資産番号,場所,部署,メモ");

            foreach (var r in list)
            {
                sb.AppendLine($"{Esc(r.Date)},{Esc(r.TrackType)},{Esc(r.DeviceName)},{Esc(r.AssetTag)},{Esc(r.Location)},{Esc(r.Dept)},{Esc(r.Note)}");
            }

            // ★ Shift_JIS(CP932) でエンコード（日本語Excelで安全）
            var sjis = Encoding.GetEncoding(932);
            var bytes = sjis.GetBytes(sb.ToString());

            var fileName = $"expiry_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(bytes, "text/csv; charset=shift_jis", fileName);
        }
    }
}
