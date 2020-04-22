using Caliburn.Micro;
using Common;
using System;
using System.Diagnostics;
using System.Linq;

namespace Viewer.ViewModels
{
    internal class FilterControlViewModel : Screen
    {
        private readonly IVideoRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        private string _selectedCategory;
        private string _selectedAutor;

        public string Title { get; set; }

        public BindableCollection<string> Categories { get; set; } = new BindableCollection<string>();

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public BindableCollection<string> Authors { get; set; } = new BindableCollection<string>();

        public string SelectedAuthor
        {
            get => _selectedAutor;
            set
            {
                if (_selectedAutor != value)
                {
                    _selectedAutor = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public FilterControlViewModel(IVideoRepository repository, IEventAggregator eventAggregator)
        {
            _repository = repository ?? throw new ArgumentNullException();
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException();
            InitFilters();
        }

        public void Search()
        {
            Debug.WriteLine(Title);
        }

        private void InitFilters()
        {
            var videos = _repository.GetAllVideos();
            var categories = videos
                .Select(v => v.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .ToList();
            Categories.Add("Все");
            Categories.AddRange(categories);
            var authors = videos
                .Select(v => v.Author)
                .Distinct()
                .ToList();
            Authors.Add("Все");
            Authors.AddRange(authors);

            SelectedCategory = Categories[0];
            SelectedAuthor = Authors[0];
        }
    }
}
