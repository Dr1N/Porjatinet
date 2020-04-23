using Caliburn.Micro;
using Common.Model;
using System;

namespace Viewer.ViewModels
{
    internal class VideoViewModel : PropertyChangedBase
    {
        private readonly Video _video;

        public VideoViewModel(Video video)
        {
            _video = video ?? throw new ArgumentNullException(nameof(video));
        }
    }
}