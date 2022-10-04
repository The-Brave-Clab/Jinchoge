using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    internal class MainWindowBottomToolbarViewModel : ViewModelBase
    {
        private string progressBarText = "";
        public string ProgressBarText
        {
            get => progressBarText;
            set => this.RaiseAndSetIfChanged(ref progressBarText, value);
        }

        private float toolbarProgress = 0;
        public float ToolbarProgress
        {
            get => toolbarProgress;
            set => this.RaiseAndSetIfChanged(ref toolbarProgress, value);
        }

        private bool showProgressText = false;
        public bool ShowProgressText
        {
            get => showProgressText;
            set => this.RaiseAndSetIfChanged(ref showProgressText, value);
        }

        private bool isProgressIndeterminate = false;
        public bool IsProgressIndeterminate
        {
            get => isProgressIndeterminate;
            set => this.RaiseAndSetIfChanged(ref isProgressIndeterminate, value);
        }

        private bool isProgressBarVisible = false;
        public bool IsProgressBarVisible
        {
            get => isProgressBarVisible;
            set => this.RaiseAndSetIfChanged(ref isProgressBarVisible, value);
        }

        private string toolbarText = "";
        public string ToolbarText
        {
            get => toolbarText;
            set => this.RaiseAndSetIfChanged(ref toolbarText, value);
        }



        public void ClearProgressBar()
        {
            IsProgressBarVisible = false;
            ProgressBarText = "";
            ToolbarProgress = 0;
            ShowProgressText = false;
            IsProgressIndeterminate = false;
        }

    }
}
