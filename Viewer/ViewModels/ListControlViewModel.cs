﻿using Caliburn.Micro;
using Common;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Viewer.Messages;
using Viewer.Views;

namespace Viewer.ViewModels
{
    internal class ListControlViewModel : Screen, IHandle<FilterChangedMessage>
    {
        private readonly IVideoRepository _repository;
        private readonly IEventAggregator _eventAggregator;
        private readonly List<Video> _videos;

        private VideoViewModel _selectedVideo;

        public BindableCollection<VideoViewModel> List { get; set; } = new BindableCollection<VideoViewModel>();

        public string Videos => _videos?.Count.ToString() ?? "0";

        public string Show => List.Count.ToString();

        public string SelectedVideo => List.IndexOf(_selectedVideo) != -1 ? (List.IndexOf(_selectedVideo) + 1).ToString() : "N/A";

        public string SelectedIndex => _videos.IndexOf(_selectedVideo?.Video) != -1 ? (_videos.IndexOf(_selectedVideo.Video) + 1).ToString() : "N/A";

        public bool AutoPlay { get; set; } = true;

        public string ScrollTo { get; set; } = string.Empty;

        public VideoViewModel SelectedList
        {
            get => _selectedVideo;
            set
            {
                if (value != _selectedVideo)
                {
                    _selectedVideo = value;
                    NotifyOfPropertyChange();
                    NotifyOfPropertyChange(() => SelectedVideo);
                    NotifyOfPropertyChange(() => SelectedIndex);
                    SendMessage();
                }
            }
        }

        public void Scroll()
        {
            if (int.TryParse(ScrollTo, out int scroll)
                && GetView() is ListControlView view)
            {
                scroll--;
                if (0 < scroll && scroll < view.List.Items.Count)
                {
                    if (view.List.Items[scroll] is VideoViewModel item)
                    {
                        SelectedList = item;
                        view.List.ScrollIntoView(SelectedList);
                    }
                }
            }
        }

        public ListControlViewModel(IVideoRepository repository, IEventAggregator eventAggregator)
        {
            _repository = repository ?? throw new ArgumentNullException();
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException();
            _videos = _repository.GetAllVideos().OrderByDescending(v => v.Publish).ToList();
            InitList();
            _eventAggregator.Subscribe(this);
        }

        private void InitList()
        {
            foreach (var video in _videos)
            {
                List.Add(new VideoViewModel(video));
            }
        }

        private void SendMessage()
        {
            if (_selectedVideo == null)
            {
                return;
            }
            _eventAggregator.PublishOnUIThread(new VideoSelectedMessage(_selectedVideo.Video, AutoPlay));
        }

        public void Handle(FilterChangedMessage message)
        {
            if (message == null)
            {
                return;
            }

            var filtered = _videos;
            if (!string.IsNullOrEmpty(message.Title))
            {
                filtered = filtered.Where(v => v.Title.Contains(message.Title)).ToList();
            }

            if (message.Category != "Все")
            {
                filtered = filtered.Where(v => v.Category == message.Category).ToList();
            }

            if (message.Author != "Все")
            {
                filtered = filtered.Where(v => v.Author == message.Author).ToList();
            }

            List.Clear();
            filtered.ForEach(v => List.Add(new VideoViewModel(v)));
            NotifyOfPropertyChange(() => Show);
        }
    }
}
