using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MagestyMediaPlayer.Core.Interfaces;
using MagestyMediaPlayer.Core.Models;
using Microsoft.EntityFrameworkCore;
using MonoTorrent;
using MonoTorrent.Client;
using Player.Infrastructure.Data;

namespace MagestyMediaPlayer.Infrastructure.Services
{
    public class LocalMediaRepository : IMediaRepository
    {
        private readonly AppDbContext _context;

        private readonly string[] _searchPaths;

        public LocalMediaRepository(AppDbContext context)
        {
            _context = context;
            _searchPaths =
            [
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
            ];
        }

        public async Task AddMediaItemAsync(MediaItem mediaItem)
        {
            if (mediaItem.SourceType != SourceType.Local || !File.Exists(mediaItem.SourceUri))
                throw new ArgumentException("Invalid local file path");

            var existing = await _context.MediaItems
                .FirstOrDefaultAsync(m => m.SourceUri == mediaItem.SourceUri && m.SourceType == SourceType.Local);

            if (existing == null)
            {
                mediaItem.Id = Guid.NewGuid();
                mediaItem.AddedDate = DateTime.Now;
                _context.MediaItems.Add(mediaItem);
                await _context.SaveChangesAsync();
            }
        }

        public Task DeleteMediaItemAsync(MediaItem item)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteMediaItemAsync(Guid id)
        {
            var mediaItem = await _context.MediaItems.FindAsync(id);
            if (mediaItem != null && mediaItem.SourceType == SourceType.Local)
            {
                _context.MediaItems.Remove(mediaItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(MediaItem mediaItem)
        {
            var existing = await _context.MediaItems.FindAsync(mediaItem.Id);
            if (existing == null || existing.SourceType != SourceType.Local)
                throw new InvalidOperationException("MediaItem not found or not local");

            existing.Title = mediaItem.Title;
            existing.Artist = mediaItem.Artist;
            existing.Album = mediaItem.Album;
            existing.Genre = mediaItem.Genre;
            existing.Duration = mediaItem.Duration;
            existing.Year = mediaItem.Year;
            existing.MediaType = mediaItem.MediaType;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MediaItem>> GetAllAsync()
        {
            return await _context.MediaItems
                .Where(m => m.SourceType == SourceType.Local)
                .ToListAsync();
        }

        public async Task<MediaItem> GetByIdAsync(Guid id)
        {
            return await _context.MediaItems.FirstOrDefaultAsync(m => m.Id == id && m.SourceType == SourceType.Local);
        }

        private IEnumerable<string> SearchLocalFiles(string query, string category)
        {
            var extensions = category == "101" ? new[] { ".mp3", ".flac" } : new[] { ".mp4", ".mkv" };
            var files = new List<string>();

            foreach (var path in _searchPaths)
            {
                if (!Directory.Exists(path))
                    continue;

                files.AddRange(Directory
                    .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(Path.GetExtension(f).ToLowerInvariant()) &&
                                (string.IsNullOrEmpty(query) || Path.GetFileName(f).Contains(query, StringComparison.OrdinalIgnoreCase))));
            }

            return files;
        }

        private MediaItem CreateMediaItemFromFile(string filePath)
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
                mediaItem.Year = (file.Tag.Year > 0 ? file.Tag.Year : (uint)DateTime.Now.Year);
            }
            catch
            {
                Debug.WriteLine($"Cannot reach the file: {filePath}");
            }

            return mediaItem;
        }
    }
}
