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
using Avalonia.Threading;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    public class UpdateLocalDataViewModel : ViewModelBase
    {
        private UpdateLocalData? window = null;

        public void SetWindow(UpdateLocalData window)
        {
            this.window = window;
        }

        public WindowTransparencyLevel WindowTransparency
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    if (Environment.OSVersion.Version.Build >= 22000)
                        return WindowTransparencyLevel.Mica;
                return WindowTransparencyLevel.AcrylicBlur;
            }
        }
        public IBrush WindowBrush
        {
            get
            {
                return new SolidColorBrush(Colors.Black, 0.6);
            }
        }

        private string downloadingFileName = "Downloading File";
        public string DownloadingFileName
        {
            get => downloadingFileName;
            set => this.RaiseAndSetIfChanged(ref downloadingFileName, value);
        }

        private float downloadingFileProgress = 0;
        public float DownloadingFileProgress
        {
            get => downloadingFileProgress;
            set => this.RaiseAndSetIfChanged(ref downloadingFileProgress, value);
        }

        private string currentDownloadedFilesText = "(0/0)";
        public string CurrentDownloadedFiles
        {
            get => currentDownloadedFilesText;
            set => this.RaiseAndSetIfChanged(ref currentDownloadedFilesText, value);
        }

        private float currentDownloadedProgress = 0;
        public float CurrentDownloadedProgress
        {
            get => currentDownloadedProgress;
            set => this.RaiseAndSetIfChanged(ref currentDownloadedProgress, value);
        }

        public void UpdateLocalData()
        {
            LocalData.Update(
                    (fileName, progress) =>
                    {
                        DownloadingFileName = fileName;
                        DownloadingFileProgress = progress * 100;
                    },
                    (current, total) =>
                    {
                        CurrentDownloadedFiles = $"({current} / {total})";
                        CurrentDownloadedProgress = total == 0 ? 100.0f : current * 100.0f / total;
                    }
                )
                .ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted)
                .ContinueWith(_ => Dispatcher.UIThread.Post(() => window.Close()));
        }
    }
}
