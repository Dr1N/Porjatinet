using Microsoft.Toolkit.Wpf.UI.Controls;
using System;
using System.Windows;

namespace Viewer.Helpers
{
    public static class WebBrowserHelper
    {
        [Obsolete]
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.RegisterAttached("Url", typeof(string), typeof(WebBrowserHelper),
                new PropertyMetadata(OnUrlChanged));

        [Obsolete]
        public static string GetUrl(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(UrlProperty);
        }

        [Obsolete]
        public static void SetUrl(DependencyObject dependencyObject, string body)
        {
            dependencyObject.SetValue(UrlProperty, body);
        }

        [Obsolete]
        private static void OnUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is WebView browser)) return;

            Uri uri = null;

            if (e.NewValue is string s)
            {
                var uriString = s;

                uri = string.IsNullOrWhiteSpace(uriString) ? null : new Uri(uriString);
            }
            else if (e.NewValue is Uri)
            {
                uri = (Uri)e.NewValue;
            }

            browser.Source = uri;
        }
    }
}
