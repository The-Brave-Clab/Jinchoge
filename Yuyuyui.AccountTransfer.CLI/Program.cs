using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer;

namespace Yuyuyui.AccountTransfer.CLI
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            object logLock = new();
            Utils.SetLogCallbacks(
                o =>
                {
                    lock (logLock) 
                        ColoredOutput.WriteLine(o, ConsoleColor.Green);
                },
                o =>
                {
                    lock (logLock) 
                        Console.WriteLine(o);
                },
                o =>
                {
                    lock (logLock) 
                        ColoredOutput.WriteLine(o, ConsoleColor.Yellow);
                },
                o =>
                {
                    lock (logLock) 
                        ColoredOutput.WriteLine(o, ConsoleColor.Red);
                }
            );

            await LocalData.Update();

            PrivateServer.PrivateServer.Start<AccountTransferProxyCallbacks>(endpoint =>
            {
                //foreach (var endPoint in proxyServer.ProxyEndPoints)
                Console.Write("Listening at ");
                ColoredOutput.WriteLine($"{endpoint.IpAddress}:{endpoint.Port}", ConsoleColor.Cyan);

                Console.WriteLine();

                ColoredOutput.WriteLine("Please use one of the following addresses as your proxy:", ConsoleColor.Yellow);

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

                        ColoredOutput.Write($"{addr.Address}:{endpoint.Port}", ConsoleColor.Green);
                        Console.Write("\t");
                        ColoredOutput.Write(netInterface.Name, ConsoleColor.DarkMagenta);
                        Console.Write(", ");
                        ColoredOutput.WriteLine(netInterface.Description, ConsoleColor.DarkBlue);
                    }
                }

                Console.WriteLine();

                return endpoint;
            });

            TransferProgress.WaitForCompletion();
            
            ColoredOutput.Write("All transfer tasks have completed. Program will exit in 10 seconds.", ConsoleColor.Green);
            Thread.Sleep(10000);

            PrivateServer.PrivateServer.Stop<AccountTransferProxyCallbacks>();
        }
    }
}