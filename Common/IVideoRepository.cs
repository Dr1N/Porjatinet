using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model;

namespace Common
{
    public interface IVideoRepository
    {
        void Add(Video video);

        void Remove(Video video);

        void Remove(string url);

        Video GetVideo(string url);

        List<Video> GetAllVideos();

        void Clear();

        Task SaveChangesAsync();
    }
}