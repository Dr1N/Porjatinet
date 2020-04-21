using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Exception;
using Common.Model;
using Newtonsoft.Json;

namespace Common
{
    public class JsonVideoRepository : IVideoRepository
    {
        private readonly string _fileName;
        private readonly HashSet<Video> _videos = new HashSet<Video>();

        public JsonVideoRepository(string fileName = "videos.json")
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            _fileName = Path.GetFullPath(fileName);
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

        public void Remove(Video video)
        {
            if (video == null)
            {
                return;
            }

            _videos.Remove(video);
        }

        public void Remove(string url)
        {
            var video = _videos.FirstOrDefault(v => v.VideoUrl == url);
            Remove(video);
        }

        public Video GetVideo(string url)
        {
            return _videos.FirstOrDefault(v => string.Compare(v.VideoUrl, url, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public List<Video> GetAllVideos()
        {
            return _videos.ToList();
        }

        public void Clear()
        {
            _videos.Clear();
        }

        public void SaveChanges()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_videos, Formatting.Indented);
                File.WriteAllText(_fileName, json);
            }
            catch (System.Exception e)
            {
                throw new RepositoryException($"Saving error: {e.Message}");
            }
        }

        private void Initialize()
        {
            if (!File.Exists(_fileName)) return;

            try
            {
                var content = File.ReadAllText(_fileName);
                if (!string.IsNullOrEmpty(content))
                {
                    var videos = JsonConvert.DeserializeObject<List<Video>>(content);
                    videos.ForEach(v => _videos.Add(v));
                }
            }
            catch (System.Exception e)
            {
                throw new RepositoryException($"Initialize error: {e.Message}");
            }
        }
    }
}