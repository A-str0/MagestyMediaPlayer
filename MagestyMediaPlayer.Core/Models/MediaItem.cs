using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; }
        public string? Artist { get; set; }
        public string? Album { get; set; }
        public string? Genre { get; set; }

        public TimeSpan? Duration { get; set; }
        public uint? Year { get; set; }

        public string SourceUri { get; set; }
        public string FileName { get; set; }

        public MediaType MediaType { get; set; }
        public SourceType SourceType { get; set; }
        public DateTime AddedDate { get; set; }

        public virtual ICollection<PlaylistItem> PlaylistItems { get; set; } = new List<PlaylistItem>();

        public virtual void Print()
        {
            Debug.WriteLine($"({Id}) {Title} - {Artist} | {Album}, {Genre}; URI: {SourceUri}; Added to DB: {AddedDate}");
        }
    }
}
