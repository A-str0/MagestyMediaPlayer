using System.Diagnostics;
using ZLinq;
using MagestyMediaPlayer.Core.Interfaces;
using MagestyMediaPlayer.Core.Models;
using MagestyMediaPlayer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MagestyMediaPlayer.Infrastructure.Services
{
    public class LocalMediaRepository : IMediaRepository
    {
        private IDbContextFactory<AppDbContext> _contextFactory;

        private readonly string[] _searchPaths;

        public LocalMediaRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _searchPaths =
            [
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
            ]; // TODO: make it customizable
        }

        public async Task AddMediaItemAsync(MediaItem mediaItem)
        {
            Debug.WriteLine($"Adding MediaItem {mediaItem.Id} to DB");

            if (mediaItem.SourceType != SourceType.Local || !File.Exists(mediaItem.SourceUri))
                throw new ArgumentException("Invalid local file path");

            using var context = _contextFactory.CreateDbContext();

            var existing = await context.MediaItems
                .FirstOrDefaultAsync(m => m.SourceUri == mediaItem.SourceUri && m.SourceType == SourceType.Local);

            if (existing == null)
            {
                mediaItem.Id = Guid.NewGuid();
                mediaItem.AddedDate = DateTime.Now;

                Debug.WriteLine($"Adding file {mediaItem.FileName} to DB");
                mediaItem.Print();

                context.MediaItems.Add(mediaItem);
                await context.SaveChangesAsync();

                Debug.WriteLine($"{mediaItem.FileName} was added to DB");
            }
            else
            {
                Debug.WriteLine($"MediaItem {mediaItem} exists: {existing}");
            }
        }

        public Task DeleteMediaItemAsync(MediaItem item)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteMediaItemAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();

            var mediaItem = await context.MediaItems.FindAsync(id);
            if (mediaItem != null && mediaItem.SourceType == SourceType.Local)
            {
                context.MediaItems.Remove(mediaItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(MediaItem mediaItem)
        {
            using var context = _contextFactory.CreateDbContext();

            var existing = await context.MediaItems.FindAsync(mediaItem.Id);
            if (existing == null || existing.SourceType != SourceType.Local)
                throw new InvalidOperationException("MediaItem not found or not local");

            existing.Title = mediaItem.Title;
            existing.Artist = mediaItem.Artist;
            existing.Album = mediaItem.Album;
            existing.Genre = mediaItem.Genre;
            existing.Duration = mediaItem.Duration;
            existing.Year = mediaItem.Year;
            existing.MediaType = mediaItem.MediaType;

            await context.SaveChangesAsync();
        }

        public async Task<MediaItem?> GetByIdAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.MediaItems.FirstOrDefaultAsync(m => m.Id == id && m.SourceType == SourceType.Local);
        }

        public async Task<IEnumerable<MediaItem>> GetAllAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.MediaItems
                .Where(m => m.SourceType == SourceType.Local)
                .ToListAsync();
        }

        public IEnumerable<string> SearchLocalFiles(string query, string category)
        {
            var extensions = category == "101" ? new[] { ".mp3", ".flac", ".wav" } : new[] { ".mp4", ".mkv" };
            var files = new List<string>();

            foreach (var path in _searchPaths)
            {
                if (!Directory.Exists(path))
                {
                    Debug.WriteLine($"{this}: Directory \'{path}\' does not exists!", "warn");
                    continue;
                }

                files.AddRange(Directory
                    .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(Path.GetExtension(f).ToLowerInvariant()) &&
                                (string.IsNullOrEmpty(query) || Path.GetFileName(f).Contains(query, StringComparison.OrdinalIgnoreCase))));
            }

            return files;
        }

        public MediaItem CreateMediaItemFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception($"File does not exists: {filePath}!");

            var mediaItem = new MediaItem
            {
                Id = Guid.NewGuid(),
                SourceUri = filePath,
                FileName = Path.GetFileName(filePath),
                Title = Path.GetFileNameWithoutExtension(filePath),
                SourceType = SourceType.Local,
                MediaType = filePath.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ? MediaType.Video : MediaType.Audio,
                AddedDate = DateTime.Now
            };

            try
            {
                using var file = TagLib.File.Create(filePath);
                mediaItem.Title = file.Tag.Title ?? mediaItem.Title;
                mediaItem.Artist = file.Tag.FirstPerformer;
                mediaItem.Album = file.Tag.Album;
                mediaItem.Genre = file.Tag.FirstGenre;
                mediaItem.Duration = file.Properties.Duration;
                mediaItem.Year = file.Tag.Year > 0 ? file.Tag.Year : (uint)DateTime.Now.Year;
            }
            catch
            {
                Debug.WriteLine($"{this}: Cannot reach the file: {filePath}", "warn");
            }

            return mediaItem;
        }
    }
}
