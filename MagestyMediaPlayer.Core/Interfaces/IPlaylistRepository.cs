using MagestyMediaPlayer.Core.Models;

namespace MagestyMediaPlayer.Core.Interfaces
{
    public interface IPlaylistRepository
    {
        Task<Playlist> CreatePlaylistAsync(string name, string description = "");
        Task DeletePlaylistAsync(Guid id);

        Task AddTrackToPlaylistAsync(Guid playlistId, Guid mediaItemId);
        Task AddTracksToPlaylistAsync(Guid playlistId, IEnumerable<Guid> mediaItemIds);
        Task RemoveTrackFromPlaylistAsync(Guid playlistId, Guid mediaItemId);

        Task ReorderTrackAsync(Guid playlistId, Guid mediaItemId, int newOrder); 

        Task<Playlist?> GetPlaylistAsync(Guid id);
        Task<IEnumerable<Playlist>> GetAllPlaylistsAsync();
        // Task LoadPlaylistIntoQueueAsync(Guid playlistId, bool shuffle = false);
    }
}