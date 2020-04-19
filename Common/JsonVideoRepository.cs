using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Exception;
using Common.Model;
using Newtonsoft.Json;

namespace Common
{
    public class JsonVideoRepository : IVideoRepository
    {
        private readonly string _fileName;
        private readonly HashSet<Video> _videos;
        
        public JsonVideoRepository(string fileName = "videos.json")
        {
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            _videos = new HashSet<Video>();
            Initialize();
        }
        
        public void Add(Video video)
        {
            if (video == null)
            {
                throw new ArgumentNullException(nameof(video));
            }
            
            _videos.Add(video);
        }

        public void Edit(string url, Video video)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(nameof(url));
            }

            if (video == null)
            {
                throw new ArgumentNullException(nameof(video));
            }
            
            throw new NotImplementedException();
        }

        public void Remove(Video video)
        {
            throw new NotImplementedException();
        }

        public void Remove(string url)
        {
            throw new NotImplementedException();
        }

        public Video GetVideo(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(nameof(url));
            }

            return _videos.FirstOrDefault(v => string.Compare(v.VideoUrl, url, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public List<Video> GetAllVideos()
        {
            return _videos.ToList();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        private void Initialize()
        {
            if (!File.Exists(_fileName)) return;
            
            try
            {    
                var content = File.ReadAllText(_fileName);
                var videos = JsonConvert.DeserializeObject<List<Video>>(content);
                videos.ForEach(v => _videos.Add(v));
            }
            catch (System.Exception e)
            {
                throw new RepositoryException(e.Message);
            }
        }
    }
}