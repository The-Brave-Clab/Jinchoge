using System.Net;
using System.Net.Sockets;

namespace Yuyuyui.PrivateServer
{
    public class HttpServer
    {
        private int listeningPort;
        public int Port => listeningPort;

        private bool running;
        private Task? listenTask;

        private TcpListener listener;

        

        public HttpServer(int port)
        {
            listeningPort = port;
            
            listener = new TcpListener(IPAddress.Any, port);

            running = false;
            listenTask = null;
        }

        public void Start()
        {
            running = true;
            
            listener.Start();
            
            listenTask = Listen();
        }

        private async Task Listen()
        {
            while (running)
            {    
                using (var tcpClient = await listener.AcceptTcpClientAsync())
                using (var stream = tcpClient.GetStream())
                using (var reader = new StreamReader(stream))
                using (var writer = new StreamWriter(stream))
                {
                    // ヘッダー部分を全部読む
                    string line;
                    do
                    {
                        line = await reader.ReadLineAsync();
                        // 読んだ行を出力しておく
                        Console.WriteLine(line);
                    } while (!string.IsNullOrWhiteSpace(line));
                }
                
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            
        }

        public void Stop()
        {
            running = false;
            
            listener.Stop();
        }
    }
}