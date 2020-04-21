using System;

namespace Common.Model
{
    public class Video
    {
        public string VideoUrl { get; set; }

        public string PostUrl { get; set; }

        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }

        public DateTime Publish { get; set; }

        public DateTime Parsed { get; set; }

        public Video()
        {
        }

        public Video(string url)
        {
            VideoUrl = url ?? throw new ArgumentNullException(nameof(url));
        }

        public override bool Equals(object obj)
        {
            if (obj is Video other)
            {
                return VideoUrl == other.VideoUrl;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return VideoUrl.GetHashCode();
        }
    }
}