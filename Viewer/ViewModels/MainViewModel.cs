using Caliburn.Micro;
using System;

namespace Viewer.ViewModels
{
    internal class MainViewModel : Screen
    {
        public FilterControlViewModel FilterControl { get; set; }

        public ListControlViewModel ListControl { get; set; }

        public ViewControlViewModel ViewControl { get; set; }

        public MainViewModel(
            FilterControlViewModel filterControlViewModel,
            ListControlViewModel listControlViewModel,
            ViewControlViewModel viewControlViewModel)
        {
            FilterControl = filterControlViewModel ?? throw new ArgumentNullException();
            ListControl = listControlViewModel ?? throw new ArgumentNullException();
            ViewControl = viewControlViewModel ?? throw new ArgumentNullException();
        }
    }
}
