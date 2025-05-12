using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MagestyMediaPlayer.Core.Models
{
    public class Playlist
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)] 
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ICollection<PlaylistItem> PlaylistItems { get; set; } = new List<PlaylistItem>();
    }
}