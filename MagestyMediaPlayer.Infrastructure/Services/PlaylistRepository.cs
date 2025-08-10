using MagestyMediaPlayer.Core.Interfaces;
using MagestyMediaPlayer.Core.Models;
using MagestyMediaPlayer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MagestyMediaPlayer.Infrastructure.Services
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private IDbContextFactory<AppDbContext> _contextFactory;

        public PlaylistRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Playlist> CreatePlaylistAsync(string name, string description = "")
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            Playlist playlist = new Playlist
            {
                Name = name,
                Description = description,
                CreatedDate = DateTime.UtcNow
            };

            context.Playlists.Add(playlist);

            await context.SaveChangesAsync();

            return playlist;
        }

        public async Task DeletePlaylistAsync(Guid id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var playlist = await context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                context.Playlists.Remove(playlist);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddTrackToPlaylistAsync(Guid playlistId, Guid mediaItemId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            Playlist? playlist = await GetPlaylistAsync(playlistId);
            if (playlist == null) throw new KeyNotFoundException("Playlist not found");

            // Check for duplicates
            // TODO: change to HashSet
            if (playlist.PlaylistItems.Any(p => p.MediaItemId == mediaItemId))
                return;

            int maxOrder = playlist.PlaylistItems.Count != 0 ? playlist.PlaylistItems.Max(p => p.Order) : 0;

            var playlistItem = new PlaylistItem
            {
                PlaylistId = playlistId,
                MediaItemId = mediaItemId,
                Order = maxOrder + 1
            };

            playlist.PlaylistItems.Add(playlistItem);
            context.PlaylistItems.Add(playlistItem);

            await context.SaveChangesAsync();
        }

        public async Task AddTracksToPlaylistAsync(Guid playlistId, IEnumerable<Guid> mediaItemIds)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            Playlist? playlist = await GetPlaylistAsync(playlistId);
            if (playlist == null) throw new KeyNotFoundException("Playlist not found");

            foreach (Guid mediaItemId in mediaItemIds)
            {
                // Check for duplicates
                // TODO: change to HashSet
                if (playlist.PlaylistItems.Any(p => p.MediaItemId == mediaItemId))
                    continue;

                int maxOrder = playlist.PlaylistItems.Count != 0 ? playlist.PlaylistItems.Max(p => p.Order) : 0;

                var playlistItem = new PlaylistItem
                {
                    PlaylistId = playlistId,
                    MediaItemId = mediaItemId,
                    Order = maxOrder + 1
                };

                playlist.PlaylistItems.Add(playlistItem);
                context.PlaylistItems.Add(playlistItem);
            }

            await context.SaveChangesAsync();
        }

        public async Task RemoveTrackFromPlaylistAsync(Guid playlistId, Guid mediaItemId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            Playlist? playlist = await GetPlaylistAsync(playlistId);
            if (playlist == null) throw new KeyNotFoundException("Playlist not found");

            var itemToRemove = playlist.PlaylistItems.FirstOrDefault(p => p.MediaItemId == mediaItemId);
            if (itemToRemove != null)
            {
                playlist.PlaylistItems.Remove(itemToRemove);
                context.PlaylistItems.Remove(itemToRemove);
                RebuildOrders(playlist.PlaylistItems);

                await context.SaveChangesAsync();
            }
        }

        public async Task ReorderTrackAsync(Guid playlistId, Guid mediaItemId, int newOrder)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            Playlist? playlist = await GetPlaylistAsync(playlistId);
            if (playlist == null) throw new KeyNotFoundException("Playlist not found");

            var itemToReorder = playlist.PlaylistItems.FirstOrDefault(pi => pi.MediaItemId == mediaItemId);
            if (itemToReorder == null) throw new KeyNotFoundException("Track not found in playlist");

            itemToReorder.Order = newOrder;
            RebuildOrders(playlist.PlaylistItems);

            await context.SaveChangesAsync();
        }

        public async Task<Playlist?> GetPlaylistAsync(Guid id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Playlists
                .Include(p => p.PlaylistItems)
                .ThenInclude(p => p.MediaItem)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Playlist>> GetAllPlaylistsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Playlists
                .Include(p => p.PlaylistItems)
                .ThenInclude(pi => pi.MediaItem)
                .ToListAsync();
        }

        private void RebuildOrders(ICollection<PlaylistItem> items)
        {
            var sortedItems = items.OrderBy(pi => pi.Order).ToList();
            for (int i = 0; i < sortedItems.Count; i++)
            {
                sortedItems[i].Order = i + 1;
            }
            items = sortedItems;
        }
    }
}