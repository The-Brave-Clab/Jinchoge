using System;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer.CLI
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CliOptions options = CliOptions.Parse(args);

            Config.Load();
            var cultureInfo = CultureInfo.GetCultureInfo(
                Config.ResolveInterfaceCultureName(Config.Get().General.Language));
            if (!cultureInfo.Equals(CultureInfo.InvariantCulture))
                Thread.CurrentThread.CurrentUICulture = cultureInfo;

            if (options.ShowHelp)
            {
                PrintHelp();
                return;
            }

            object logLock = new();
            Utils.SetLogCallback(
                (o, t) =>
                {
                    lock (logLock)
                    {
                        switch (t)
                        {
                            case Utils.LogType.Trace:
                                ColoredOutput.WriteLine(o, ConsoleColor.Green);
                                break;
                            case Utils.LogType.Info:
                                Console.WriteLine(o);
                                break;
                            case Utils.LogType.Warning:
                                ColoredOutput.WriteLine(o, ConsoleColor.Yellow);
                                break;
                            case Utils.LogType.Error:
                                ColoredOutput.WriteLine(o, ConsoleColor.Red);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(t), t, null);
                        }
                    }
                }
            );

            Utils.Log(string.Format(Resources.LOG_VERSION,
                Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
                    .InformationalVersion));

            if (options.ShowVersion)
                return;

            await LocalData.Update();

            if (options.UpdateDataOnly)
                return;

            var endpoint = Proxy<PrivateServerProxyCallbacks>.Start(options.Port);

            //foreach (var endPoint in proxyServer.ProxyEndPoints)
            Console.Write(Resources.LOG_LISTENING_AT);
            ColoredOutput.WriteLine($"{endpoint.IpAddress}:{endpoint.Port}", ConsoleColor.Cyan);

            Console.WriteLine();

            ColoredOutput.WriteLine(Resources.PS_STATUS_CHOOSE_MULTIPLE, ConsoleColor.Yellow);

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
                    Console.Write(@"	");
                    ColoredOutput.Write(netInterface.Name, ConsoleColor.DarkMagenta);
                    Console.Write(@", ");
                    ColoredOutput.WriteLine(netInterface.Description, ConsoleColor.DarkBlue);
                }
            }

            Console.WriteLine();
            
            Console.WriteLine(Resources.LOG_PS_ENTER_TO_EXIT);
            
            Console.WriteLine();

            Console.ReadLine();

            Proxy<PrivateServerProxyCallbacks>.Stop();
        }

        private static void PrintHelp()
        {
            string executable = Assembly.GetExecutingAssembly().GetName().Name ?? "Jinchoge.CLI";

            Console.WriteLine($"{executable} [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -h, --help              Show this help text and exit.");
            Console.WriteLine("  -v, --version           Show the application version and exit.");
            Console.WriteLine("  -p, --port <port>       Listen on the specified proxy port. Defaults to 44460.");
            Console.WriteLine("      --update-data-only  Update local game data, then exit without starting the proxy.");
        }

        private class CliOptions
        {
            public bool ShowHelp { get; private set; }
            public bool ShowVersion { get; private set; }
            public bool UpdateDataOnly { get; private set; }
            public int Port { get; private set; } = 44460;

            public static CliOptions Parse(string[] args)
            {
                CliOptions options = new();

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-h":
                        case "--help":
                            options.ShowHelp = true;
                            break;
                        case "-v":
                        case "--version":
                            options.ShowVersion = true;
                            break;
                        case "--update-data-only":
                            options.UpdateDataOnly = true;
                            break;
                        case "-p":
                        case "--port":
                            if (i + 1 >= args.Length ||
                                !int.TryParse(args[++i], out int port) ||
                                port < IPEndPoint.MinPort ||
                                port > IPEndPoint.MaxPort)
                                throw new ArgumentException("The port option requires a value from 0 to 65535.");

                            options.Port = port;
                            break;
                        default:
                            throw new ArgumentException($"Unknown option: {args[i]}");
                    }
                }

                return options;
            }
        }
    }
}
