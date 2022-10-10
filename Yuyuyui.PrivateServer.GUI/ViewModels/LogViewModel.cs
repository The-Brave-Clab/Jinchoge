using System;
using System.Collections.ObjectModel;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    internal class LogViewModel : ViewModelBase
    {
        public ObservableCollection<LogEntry> Logs { get; }

        public LogViewModel()
        {
            Logs = new ObservableCollection<LogEntry>();
        }
    }
    public class LogEntry
    {
        public Utils.LogType LogType { get; set; }
        public string LogContent { get; set; } = "";
        public string CreatedTime { get; set; } = $"[{DateTime.Now:HH:mm:ss}]";
    }
}
