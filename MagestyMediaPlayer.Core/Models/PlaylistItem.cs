using System.ComponentModel.DataAnnotations;

namespace MagestyMediaPlayer.Core.Models
{
    public class PlaylistItem
    {
        public Guid Id { get; set; }

        public Guid PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }

        [Required] public Guid MediaItemId { get; set; }
        public virtual MediaItem MediaItem { get; set; }

        public int Order { get; set; } // TODO: define the order of items in the playlist
    }
}