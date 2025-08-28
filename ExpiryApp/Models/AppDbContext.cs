using Microsoft.EntityFrameworkCore;

namespace ExpiryApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Device> Devices => Set<Device>();
        public DbSet<ExpiryTrack> ExpiryTracks => Set<ExpiryTrack>();

        // ★ ここを新規追加（既にあれば中身だけ追記）
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AssetTag は任意入力（NULL可）
            modelBuilder.Entity<Device>()
                .Property(d => d.AssetTag)
                .IsRequired(false)        // 必須解除
                .HasMaxLength(100);       // 長さは任意。nvarchar(max) で運用ならこの行は削除OK

            // ★ AssetTag にユニークインデックス（NULLは対象外）
            modelBuilder.Entity<Device>()
                .HasIndex(d => d.AssetTag)
                .IsUnique()
                .HasFilter("[AssetTag] IS NOT NULL"); // SQL Server 用フィルタ
        }
    }
}
