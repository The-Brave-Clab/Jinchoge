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
        private bool shouldScrollToBottom;
        private double lastOffsetY;

        public ConsolePage()
        {
            InitializeComponent();

            shouldScrollToBottom = true;
            lastOffsetY = 0;
        }

        private void ScrollViewerOnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            double currentOffsetY = scrollViewer!.Offset.Y;

            // Only scroll to bottom when the extent changed. Otherwise you can't scroll up
            if (e.ExtentDelta.Y != 0)
            {
                if (shouldScrollToBottom)
                    scrollViewer?.ScrollToEnd();
            }
            else if (e.ExtentDelta.Y == 0)
            {
                if (currentOffsetY > lastOffsetY)
                {
                    shouldScrollToBottom = Math.Abs(currentOffsetY + scrollViewer.Viewport.Height - scrollViewer.Extent.Height) < 5.0;
                }
                else
                {
                    shouldScrollToBottom = false;
                }
            }

            lastOffsetY = currentOffsetY;
        }
    }
}
