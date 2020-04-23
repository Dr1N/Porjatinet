using Caliburn.Micro;
using Common;
using System;
using System.Collections.Generic;
using System.Windows;
using Viewer.ViewModels;

namespace Viewer
{
    internal class AppBootstrapper : BootstrapperBase
    {
        private const string DataPath = @"Data\videos.json";
        private readonly SimpleContainer _container = new SimpleContainer();

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.Instance(_container);
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.RegisterInstance(typeof(IVideoRepository), "videorepository", new JsonVideoRepository(DataPath));

            _container.RegisterSingleton(typeof(MainViewModel), "main", typeof(MainViewModel));
            _container.RegisterSingleton(typeof(FilterControlViewModel), "filter", typeof(FilterControlViewModel));
            _container.RegisterSingleton(typeof(ListControlViewModel), "list", typeof(ListControlViewModel));
            _container.RegisterSingleton(typeof(ViewControlViewModel), "view", typeof(ViewControlViewModel));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return _container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}