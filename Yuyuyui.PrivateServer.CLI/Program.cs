using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Yuyuyui.PrivateServer.CLI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Utils.SetLogCallbacks(
                o => { ColoredOutput.WriteLine(o, ConsoleColor.Green); },
                Console.WriteLine,
                o => { ColoredOutput.WriteLine(o, ConsoleColor.Yellow); },
                o => { ColoredOutput.WriteLine(o, ConsoleColor.Red); }
            );

            var endpoint = Proxy.StartProxy();

            //foreach (var endPoint in proxyServer.ProxyEndPoints)
            Console.Write("Listening at ");
            ColoredOutput.WriteLine($"{endpoint.IpAddress}:{endpoint.Port}", ConsoleColor.Cyan);

            HttpServer httpServer = new HttpServer(44461);
            httpServer.Start();
            Console.Write("Http Server Listening at ");
            ColoredOutput.WriteLine($"{httpServer.Port}", ConsoleColor.Cyan);
            

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

                    ColoredOutput.Write($"{addr.Address}:{Proxy.DEFAULT_PORT}", ConsoleColor.Green);
                    Console.Write("\t");
                    ColoredOutput.Write(netInterface.Name, ConsoleColor.DarkMagenta);
                    Console.Write(", ");
                    ColoredOutput.WriteLine(netInterface.Description, ConsoleColor.DarkBlue);
                }
            }

            Console.WriteLine();

            Console.Read();

            Proxy.Stop();
            httpServer.Stop();
        }
    }
}