using Caliburn.Micro;
using Common.Model;
using System;

namespace Viewer.ViewModels
{
    internal class VideoViewModel : PropertyChangedBase
    {
        private readonly Video _video;

        public string Image => _video.ImageUrl;

        public string Title => _video.Title;

        public string Category => _video.Category;

        public string Author => _video.Author;

        public string Publish => $"{_video.Publish.ToShortDateString()} {_video.Publish.ToShortTimeString()}";

        public VideoViewModel(Video video)
        {
            _video = video ?? throw new ArgumentNullException(nameof(video));
        }
    }
}