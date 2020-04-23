using Caliburn.Micro;
using Common.Model;
using System;

namespace Viewer.ViewModels
{
    internal class VideoViewModel : PropertyChangedBase
    {
        public Video Video { get; }

        public string Image => Video.ImageUrl;

        public string Title => Video.Title;

        public string Category => Video.Category;

        public string Author => Video.Author;

        public string Publish => $"{Video.Publish.ToShortDateString()} {Video.Publish.ToShortTimeString()}";

        public VideoViewModel(Video video)
        {
            Video = video ?? throw new ArgumentNullException(nameof(video));
        }
    }
}