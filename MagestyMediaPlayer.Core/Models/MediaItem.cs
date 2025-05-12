using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagestyMediaPlayer.Core.Models
{
    public enum MediaType
    {
        Audio,
        Video
    }

    public enum SourceType
    {
        Local,
        Torrent
    }

    public class MediaItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required] 
        [StringLength(200)] public string Title { get; set; }
        [StringLength(100)] public string Artist { get; set; }
        [StringLength(100)] public string Album { get; set; }
        [StringLength(50)] public string Genre { get; set; }

        public TimeSpan? Duration { get; set; }
        private UInt16 Year { get; set; }

        [StringLength(500)] public string SourceUri { get; set; }
        [StringLength(200)] public string FileName { get; set; }


        public MediaType MediaType { get; set; }
        public SourceType SourceType { get; set; }
        public DateTime AddedDate { get; set; }

        public virtual ICollection<PlaylistItem> PlaylistItems { get; set; } = new List<PlaylistItem>();
    }
}
