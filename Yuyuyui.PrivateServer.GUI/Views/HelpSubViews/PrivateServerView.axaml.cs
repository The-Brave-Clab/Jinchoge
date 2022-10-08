using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Yuyuyui.PrivateServer.GUI.Views.HelpSubViews
{
    public partial class PrivateServerView : HelpSubViewBase
    {
        public PrivateServerView()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            HelpViewModel!.PushPage(typeof(GeneralInfoView));
        }
    }
}