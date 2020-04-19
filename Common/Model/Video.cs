using System;

namespace Common.Model
{
    public class Video : IEquatable<Video>
    {
        public string PageUrl {get; set; }

        public string VideoUrl { get; set; }
        
        public  string ImageUrl { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public  string Category { get; set; }
        
        public  string Author { get; set; }
        
        public  DateTime Publish { get; set; }
        
        public  DateTime Parsed { get; set; }
        
        public bool Equals(Video other)
        {
            return VideoUrl == other?.VideoUrl;
        }
    }
}