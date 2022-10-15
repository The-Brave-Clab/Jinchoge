using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.AccountTransfer.CLI
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Config.Load();
            var cultureInfo = CultureInfo.GetCultureInfo(Config.Get().General.Language);
            if (!cultureInfo.Equals(CultureInfo.InvariantCulture))
                Thread.CurrentThread.CurrentUICulture = cultureInfo;

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

            await LocalData.Update();

            EventWaitHandle waitHandle = new AutoResetEvent(false);

            TransferProgress.RegisterTaskCompleteCallback((type, progress) =>
                Utils.LogTrace(string.Format(Resources.LOG_AT_TRANSFER_SUCCESS, TransferProgress.TaskName[type]) + 
                               $" ({progress.Count(b => b)}/{progress.Length})")
            );
            
            TransferProgress.RegisterAllTaskCompleteCallback(() => waitHandle.Set());

            var endpoint = Proxy<AccountTransferProxyCallbacks>.Start();

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

            waitHandle.WaitOne();

            Proxy<AccountTransferProxyCallbacks>.Stop();
            
            ColoredOutput.Write(Resources.LOG_AT_FINISH_EXIT, ConsoleColor.Green);
            Thread.Sleep(10000);
        }
    }
}