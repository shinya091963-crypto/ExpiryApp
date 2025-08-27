using Microsoft.EntityFrameworkCore;

namespace ExpiryApp.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<ExpiryTrack> ExpiryTracks => Set<ExpiryTrack>();
}
