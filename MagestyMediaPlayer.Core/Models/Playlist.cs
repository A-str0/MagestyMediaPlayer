using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MagestyMediaPlayer.Core.Models
{
    public class Playlist
    {
        public Guid Id { get; set; } // TODO: generate Guid

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<PlaylistItem> PlaylistItems { get; set; } = new List<PlaylistItem>();
    }
}