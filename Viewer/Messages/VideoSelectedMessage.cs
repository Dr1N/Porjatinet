using Common.Model;
using System;

namespace Viewer.Messages
{
    internal class VideoSelectedMessage
    {
        public Video Video { get; }

        public bool AutoPlay { get; set; }

        public VideoSelectedMessage(Video video, bool autoPlay)
        {
            Video = video ?? throw new ArgumentNullException(nameof(video));
            AutoPlay = autoPlay;
        }
    }
}
