using Caliburn.Micro;
using System.Windows;

namespace Viewer
{
    internal class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //DisplayRootViewFor<ShellViewModel>();
        }
    }
}
