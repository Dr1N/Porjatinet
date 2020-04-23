using Caliburn.Micro;
using Common;
using System;
using System.Linq;

namespace Viewer.ViewModels
{
    internal class ListControlViewModel : Screen
    {
        private readonly IVideoRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        public BindableCollection<VideoViewModel> List { get; set; } = new BindableCollection<VideoViewModel>();

        public ListControlViewModel(IVideoRepository repository, IEventAggregator eventAggregator)
        {
            _repository = repository ?? throw new ArgumentNullException();
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException();
            InitList();
        }

        private void InitList()
        {
            foreach (var video in _repository.GetAllVideos().OrderByDescending(v => v.Publish))
            {
                List.Add(new VideoViewModel(video));
            }
        }
    }
}
