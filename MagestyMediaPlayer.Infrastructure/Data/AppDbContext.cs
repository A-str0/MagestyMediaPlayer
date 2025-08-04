using MagestyMediaPlayer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace MagestyMediaPlayer.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistItem> PlaylistItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            Console.WriteLine("Configuring ", this);

            // TODO: do directory auto-creation
            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MagestyMediaPlayer",
                "database.db"
            );
            options.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Playlist
            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.Property(p => p.Id).ValueGeneratedNever(); // Use GUID provided by app
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(500); // Optional max length
                entity.Property(p => p.CreatedDate).HasDefaultValueSql("datetime('now')");
            });

            // Configure MediaItem
            modelBuilder.Entity<MediaItem>(entity =>
            {
                entity.Property(m => m.Id).ValueGeneratedNever(); // Use GUID provided by app
                entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Artist).HasMaxLength(100);
                entity.Property(m => m.Album).HasMaxLength(100);
                entity.Property(m => m.Genre).HasMaxLength(50);
                entity.Property(m => m.SourceUri).IsRequired().HasMaxLength(500);
                entity.Property(m => m.FileName).IsRequired().HasMaxLength(200);
                entity.Property(m => m.AddedDate).HasDefaultValueSql("datetime('now')");
            });

            // Configure PlaylistItem and many-to-many relationship
            modelBuilder.Entity<PlaylistItem>(entity =>
            {
                entity.Property(pi => pi.Id).ValueGeneratedNever(); // Optional, can be auto-generated
                entity.HasKey(pi => new { pi.PlaylistId, pi.MediaItemId }); // Composite key
                entity.HasOne(pi => pi.Playlist)
                    .WithMany(p => p.PlaylistItems)
                    .HasForeignKey(pi => pi.PlaylistId);
                entity.HasOne(pi => pi.MediaItem)
                    .WithMany(m => m.PlaylistItems)
                    .HasForeignKey(pi => pi.MediaItemId);
            });

            // Configure enums as integers (if not using EF Core value converters)
            modelBuilder.Entity<MediaItem>()
                .Property(m => m.MediaType)
                .HasConversion<int>();
            modelBuilder.Entity<MediaItem>()
                .Property(m => m.SourceType)
                .HasConversion<int>();
        }
    }
}
