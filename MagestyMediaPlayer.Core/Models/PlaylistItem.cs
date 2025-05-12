using System.ComponentModel.DataAnnotations;

namespace MagestyMediaPlayer.Core.Models
{
    public class PlaylistItem
    {
        public int PlaylistId { get; set; }

        public int MediaItemId { get; set; }

        public int Order { get; set; }

        public virtual Playlist Playlist { get; set; }

        public virtual MediaItem MediaItem { get; set; }
    }
}