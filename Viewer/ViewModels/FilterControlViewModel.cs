using Caliburn.Micro;
using Common;
using System;

namespace Viewer.ViewModels
{
    internal class FilterControlViewModel : Screen
    {
        private readonly IVideoRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        public FilterControlViewModel(IVideoRepository repository, IEventAggregator eventAggregator)
        {
            _repository = repository ?? throw new ArgumentNullException();
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException();
        }
    }
}
