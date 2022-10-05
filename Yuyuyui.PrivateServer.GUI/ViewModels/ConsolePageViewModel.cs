using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.GUI.Controls;

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
        public LogEntryControl.LogType LogType { get; set; }
        public string LogContent { get; set; }
    }
}
