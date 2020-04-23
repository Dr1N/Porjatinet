using Common.Model;
using System;

namespace Viewer.Messages
{
    internal class VideoSelectedMessage
    {
        public Video Video { get; }

        public VideoSelectedMessage(Video video)
        {
            Video = video ?? throw new ArgumentNullException(nameof(video));
        }
    }
}
