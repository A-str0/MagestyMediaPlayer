using MagestyMediaPlayer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Player.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistItem> PlaylistItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            Console.WriteLine("Configuring ", this);

            // TODO: разделить для разных ОС
            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MajesticPlayer",
                "database.db"
            );
            options.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaItem>()
                .HasKey(m => m.Id);
            modelBuilder.Entity<MediaItem>()
                .Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<MediaItem>()
                .Property(m => m.Artist)
                .HasMaxLength(100);
            modelBuilder.Entity<MediaItem>()
                .Property(m => m.Album)
                .HasMaxLength(100);
            modelBuilder.Entity<MediaItem>()
                .Property(m => m.Genre)
                .HasMaxLength(50);
            modelBuilder.Entity<MediaItem>()
                .Property(m => m.SourceUri)
                .HasMaxLength(500);
            modelBuilder.Entity<MediaItem>()
                .Property(m => m.FileName)
                .HasMaxLength(500);

            modelBuilder.Entity<Playlist>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Playlist>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Playlist>()
                .Property(p => p.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<PlaylistItem>()
                .HasKey(pi => new { pi.PlaylistId, pi.MediaItemId });
            modelBuilder.Entity<PlaylistItem>()
                .HasOne(pi => pi.Playlist)
                .WithMany(p => p.PlaylistItems)
                .HasForeignKey(pi => pi.PlaylistId);
            modelBuilder.Entity<PlaylistItem>()
                .HasOne(pi => pi.MediaItem)
                .WithMany(m => m.PlaylistItems)
                .HasForeignKey(pi => pi.MediaItemId);
        }
    }
}