using System;
using System.Reactive;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using System.Runtime.InteropServices;
using Avalonia.Media;
using Yuyuyui.PrivateServer.GUI.Views;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private MainWindow? window = null;
        public void SetWindow(MainWindow window)
        { this.window = window; }

        public void OpenWindow()
        {
            var viewModel = new UpdateLocalDataViewModel();
            var newWindow = new UpdateLocalData
            {
                DataContext = viewModel
            };
            newWindow.ShowDialog(window!);

            viewModel.SetWindow(newWindow);
            viewModel.UpdateLocalData();
        }
    }
}
