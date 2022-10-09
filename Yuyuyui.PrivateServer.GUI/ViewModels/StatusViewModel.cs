using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Avalonia.Threading;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    internal class StatusViewModel : ViewModelBase
    {
        public ObservableCollection<InterfaceDescription> NetworkInterfaces { get; }

        public int ListeningPort { get; }

        public StatusViewModel()
        {
            ipMessage = "";
            NetworkInterfaces = new ObservableCollection<InterfaceDescription>();
            ListeningPort = 44460; // TODO

            UpdateNetworkInfo();

            updatingMessageVisible = false;
            notStartedMessageVisible = true;
            startedPanelVisible = false;
            currentProcess = "";
        }



        private string ipMessage;
        public string IpMessage
        {
            get => ipMessage;
            set => this.RaiseAndSetIfChanged(ref ipMessage, value);
        }

        private void UpdateNetworkInfo()
        {
            UpdateNetworkInfo(IPAddress.Any, IPAddress.Any);
        }

        private void UpdateNetworkInfo(IPAddress deviceAddress, IPAddress subnetMask)
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

                    var deviceBytes = deviceAddress.GetAddressBytes();
                    var maskBytes = subnetMask.GetAddressBytes();

                    bool isSameSubnet = true;
                    for (int i = 0; i < 4; ++i)
                    {
                        if ((bytes[i] & maskBytes[i]) != (deviceBytes[i] & maskBytes[i]))
                        {
                            isSameSubnet = false;
                            break;
                        }
                    }

                    if (!isSameSubnet) continue;

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
            else if (NetworkInterfaces.Count == 1)
            {
                IpMessage = "Use the following address as your proxy:";
            }
            else
            {
                IpMessage = "There is no address can be used as your proxy!";
            }
        }

        private bool updatingMessageVisible;
        public bool UpdatingMessageVisible
        {
            get => updatingMessageVisible;
            set => this.RaiseAndSetIfChanged(ref updatingMessageVisible, value);
        }

        private bool notStartedMessageVisible;
        public bool NotStartedMessageVisible
        {
            get => notStartedMessageVisible;
            set => this.RaiseAndSetIfChanged(ref notStartedMessageVisible, value);
        }

        private bool startedPanelVisible;
        public bool StartedPanelVisible
        {
            get => startedPanelVisible;
            set => this.RaiseAndSetIfChanged(ref startedPanelVisible, value);
        }

        private string currentProcess;
        public string CurrentProcess
        {
            get => currentProcess;
            set => this.RaiseAndSetIfChanged(ref currentProcess, value);
        }
        
        private string ipAddressInput = "";
        public string IpAddressInput
        {
            get => ipAddressInput;
            set
            {
                this.RaiseAndSetIfChanged(ref ipAddressInput, value);
                InputValid = IPAddress.TryParse(ipAddressInput, out _) && IPAddress.TryParse(subnetMaskInput, out _);
            }
        }
        
        private string subnetMaskInput = "";
        public string SubnetMaskInput
        {
            get => subnetMaskInput;
            set
            {
                this.RaiseAndSetIfChanged(ref subnetMaskInput, value);
                InputValid = IPAddress.TryParse(ipAddressInput, out _) && IPAddress.TryParse(subnetMaskInput, out _);
            }
        }

        private bool inputValid = false;

        public bool InputValid
        {
            get => inputValid;
            set => this.RaiseAndSetIfChanged(ref inputValid, value);
        }

        public void SetFilter()
        {
            UpdateNetworkInfo(IPAddress.Parse(IpAddressInput), IPAddress.Parse(SubnetMaskInput));
        }

        public void ClearFilter()
        {
            UpdateNetworkInfo();
        }

        public void SetServerStatus(MainWindowViewModel.ServerStatus status)
        {
            UpdatingMessageVisible = status == MainWindowViewModel.ServerStatus.Updating;
            NotStartedMessageVisible = status == MainWindowViewModel.ServerStatus.Stopped;
            StartedPanelVisible = status is
                MainWindowViewModel.ServerStatus.Started or
                MainWindowViewModel.ServerStatus.Transfer;
            CurrentProcess = status switch
            {
                MainWindowViewModel.ServerStatus.Updating => "",
                MainWindowViewModel.ServerStatus.Stopped => "",
                MainWindowViewModel.ServerStatus.Started => "Private Server is currently running.",
                MainWindowViewModel.ServerStatus.Transfer => "Account Transfer is in progress.",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }

    internal class InterfaceDescription
    {
        public string Socket { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
