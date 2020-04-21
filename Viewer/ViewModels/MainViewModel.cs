using Caliburn.Micro;
using System;

namespace Viewer.ViewModels
{
    internal class MainViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;

        public FilterControlViewModel FilterControl { get; set; }

        public ListControlViewModel ListControl { get; set; }

        public ViewControlViewModel ViewControl { get; set; }

        public MainViewModel(
            IEventAggregator eventAggregator,
            FilterControlViewModel filterControlViewModel,
            ListControlViewModel listControlViewModel,
            ViewControlViewModel viewControlViewModel)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException();

            FilterControl = filterControlViewModel ?? throw new ArgumentNullException();
            ListControl = listControlViewModel ?? throw new ArgumentNullException();
            ViewControl = viewControlViewModel ?? throw new ArgumentNullException();
        }
    }
}
