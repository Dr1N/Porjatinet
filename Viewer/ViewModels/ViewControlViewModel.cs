using Caliburn.Micro;
using System;
using System.Diagnostics;
using Viewer.Messages;

namespace Viewer.ViewModels
{
    internal class ViewControlViewModel : Screen, IHandle<VideoSelectedMessage>
    {
        private readonly IEventAggregator _eventAggregator;

        public string VideoSrc { get; set; }

        public string PostUrl { get; set; }

        public string VideoUrl { get; set; }

        public ViewControlViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException();
            _eventAggregator.Subscribe(this);
        }

        public void PostChrome()
        {
            Process.Start(PostUrl);
        }

        public void VideoChrome()
        {
            Process.Start(VideoUrl);
        }

        public void Handle(VideoSelectedMessage message)
        {
            if (message.AutoPlay)
            {
                VideoUrl = message.Video.VideoUrl;
                NotifyOfPropertyChange(() => VideoUrl);
            }
            PostUrl = message.Video.PostUrl;
            VideoSrc = message.Video.VideoUrl;
            NotifyOfPropertyChange(() => PostUrl);
            NotifyOfPropertyChange(() => VideoSrc);
        }
    }
}
