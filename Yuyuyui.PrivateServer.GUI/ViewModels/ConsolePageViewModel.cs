using System.Collections.ObjectModel;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    internal class ConsolePageViewModel : ViewModelBase
    {
        public ObservableCollection<LogEntry> Logs
        {
            get;
        }

        public ConsolePageViewModel()
        {
            Logs = new ObservableCollection<LogEntry>();
        }
    }
    public class LogEntry
    {
        public Utils.LogType LogType { get; set; }
        public string LogContent { get; set; } = "";
    }
}
