using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    internal class StatusPageViewModel : ViewModelBase
    {
        public ObservableCollection<InterfaceDescription> NetworkInterfaces { get; }

        public int ListeningPort { get; }

        public StatusPageViewModel()
        {
            ipMessage = "";
            NetworkInterfaces = new ObservableCollection<InterfaceDescription>();
            ListeningPort = 44460; // TODO

            UpdateNetworkInfo();

            notStartedMessageVisible = true;
            startingMessageVisible = false;
            startedPanelVisible = false;
        }



        private string ipMessage;
        public string IpMessage
        {
            get => ipMessage;
            set => this.RaiseAndSetIfChanged(ref ipMessage, value);
        }

        private void UpdateNetworkInfo()
        {
            NetworkInterfaces.Clear();

            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        continue;

                    if (addr.Address.Equals(IPAddress.Loopback))
                        continue;

                    var bytes = addr.Address.GetAddressBytes();
                    if (bytes[0] == 169 && bytes[1] == 254)
                        continue;

                    InterfaceDescription desc = new InterfaceDescription()
                    {
                        Socket = $"{addr.Address}",
                        Name = netInterface.Name,
                        Description = netInterface.Description
                    };

                    NetworkInterfaces.Add(desc);
                }
            }

            if (NetworkInterfaces.Count > 1)
            {
                IpMessage = "Use one of the following addresses as your proxy:";
            }
            else
            {
                IpMessage = "Use the following address as your proxy:";
            }
        }



        private bool notStartedMessageVisible;
        public bool NotStartedMessageVisible
        {
            get => notStartedMessageVisible;
            set => this.RaiseAndSetIfChanged(ref notStartedMessageVisible, value);
        }

        private bool startingMessageVisible;
        public bool StartingMessageVisible
        {
            get => startingMessageVisible;
            set => this.RaiseAndSetIfChanged(ref startingMessageVisible, value);
        }

        private bool startedPanelVisible;
        public bool StartedPanelVisible
        {
            get => startedPanelVisible;
            set => this.RaiseAndSetIfChanged(ref startedPanelVisible, value);
        }

        public void SetServerStatus(MainWindowViewModel.ServerStatus status)
        {
            NotStartedMessageVisible = status == MainWindowViewModel.ServerStatus.Stopped;
            StartingMessageVisible = status == MainWindowViewModel.ServerStatus.Starting;
            StartedPanelVisible = status == MainWindowViewModel.ServerStatus.Started;
        }
    }

    internal class InterfaceDescription
    {
        public string Socket { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
