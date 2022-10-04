using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using static Yuyuyui.PrivateServer.GUI.Controls.ConsolePage;

namespace Yuyuyui.PrivateServer.GUI.Controls
{
    public partial class ConsolePage : UserControl
    {

        public ConsolePage()
        {
            InitializeComponent();
        }

        private void ScrollViewerOnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            // Only scroll to bottom when the extent changed. Otherwise you can't scroll up
            if (e.ExtentDelta.Y != 0)
            {
                var scrollViewer = sender as ScrollViewer;
                scrollViewer?.ScrollToEnd();
            }
        }
    }
}
