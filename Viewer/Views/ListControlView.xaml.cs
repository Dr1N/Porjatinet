using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Viewer.Views
{
    /// <summary>
    /// Interaction logic for ListControlView.xaml
    /// </summary>
    public partial class ListControlView : UserControl
    {
        public ListControlView()
        {
            InitializeComponent();
        }

        private void ScrollTo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
