using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagestyMediaPlayer.Core.Models;

namespace MagestyMediaPlayer.Core.Interfaces
{
    public interface IMediaRepository
    {
        public Task AddMediaItemAsync(MediaItem item);
        public Task DeleteMediaItemAsync(MediaItem item);
        public Task DeleteMediaItemAsync(Guid id);
        public Task UpdateAsync(MediaItem mediaItem);
        public Task<MediaItem?> GetByIdAsync(Guid id);
        public Task<IEnumerable<MediaItem?>> GetAllAsync();
    }
}
