using System.Net;

namespace Yuyuyui.PrivateServer
{
    public class HttpServer
    {
        private int listeningPort;
        public int Port => listeningPort;
        private HttpListener listener;

        private bool running;
        private Task? listenTask;

        private HashSet<Task> requestHandlers;

        public HttpServer(int port)
        {
            listeningPort = port;
            listener = new HttpListener();
            listener.Prefixes.Add($"http://+:{listeningPort}/");

            running = true;
            listenTask = null;

            requestHandlers = new HashSet<Task>(Environment.ProcessorCount);
        }

        public void Start()
        {
            running = true;
            listener.Start();
            listenTask = Listen();
            listenTask.Start();
        }

        private async Task Listen()
        {
            for (int i = 0; i < Environment.ProcessorCount; i++)
                requestHandlers.Add(listener.GetContextAsync());

            while (running)
            {
                Task t = await Task.WhenAny(requestHandlers);
                requestHandlers.Remove(t);

                if (t is Task<HttpListenerContext> task)
                {
                    var context = task.Result;  
                    requestHandlers.Add(ProcessRequestAsync(context));
                    requestHandlers.Add(listener.GetContextAsync());
                }
                else
                {
                    t.Wait();
                }
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;
            
            EntityBase entity = EntityBase.FromHttpContext(context);
            await entity.Process();
            
            byte[] responseBody = entity.ResponseBody;
            Dictionary<string, string> responseHeaders = entity.ResponseHeaders;

            response.StatusCode = entity.GetType() == typeof(RequestErrorEntity) ? 404 : 200;

            foreach (var header in responseHeaders)
            {
                if (header.Key.Equals("Content-Type", StringComparison.CurrentCultureIgnoreCase))
                {
                    response.ContentType = header.Value;
                }
                else if (header.Key.Equals("Content-Length", StringComparison.CurrentCultureIgnoreCase))
                {
                    response.ContentLength64 = long.Parse(header.Value);
                }
                else if (header.Key.Equals("Set-Cookie", StringComparison.CurrentCultureIgnoreCase))
                {
                    var cookieDict = Utils.GetCookieDictFromString(header.Value);
                    foreach (var cookie in cookieDict)
                    {
                        response.SetCookie(new Cookie(cookie.Key,
                            string.IsNullOrEmpty(cookie.Value) ? null : cookie.Value));
                    }
                }
                else
                {
                    response.AddHeader(header.Key, header.Value);
                }
            }
            
            response.Close(responseBody, true);
        }

        public void Stop()
        {
            running = false;
            listenTask?.Wait();
            listener.Stop();
        }
    }
}