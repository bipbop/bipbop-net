using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BipbopNet.Push
{
    public class Listener
    {
        private readonly HttpListener _httpListener;
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private bool _runServer = true;

        public Listener()
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://*:{Port.ToString()}/");
            _httpListener.Start();
        }

        private static int Port
        {
            get
            {
                var environmentPort = Environment.GetEnvironmentVariable("BIPBOP_SERVER_PORT");
                if (string.IsNullOrEmpty(environmentPort)) return 8081;
                var port = int.Parse(environmentPort);
                if (port < 0 || port > 65535) return 8081;
                return port;
            }
        }

        public event EventHandler<ListenerEvent> OnRequest;

        public static async Task<string> ServerAddr()
        {
            var envData = Environment.GetEnvironmentVariable("BIPBOP_SERVER");
            if (envData != null) return envData;
            var ipRequest = await new HttpClient().GetAsync("http://ipinfo.io/ip");
            var ipResponse = await ipRequest.Content.ReadAsStringAsync();
            return $"http://{ipResponse.Trim()}:{Port}/";
        }

        public Task Stop()
        {
            var task = new Task(() =>
            {
                _runServer = false;
                _stopEvent.WaitOne();
                _httpListener.Stop();
            });
            task.Start();
            return task;
        }

        private void HandleSync()
        {
            while (_runServer) HandleConnection();
            _stopEvent.Set();
        }

        private void HandleConnection()
        {
            var context = _httpListener.GetContext();
            var response = context.Response;

            var handler = OnRequest;
            byte[] buffer;
            try
            {
                handler?.Invoke(context, new ListenerEvent(context.Request));
                buffer = Encoding.UTF8.GetBytes("OK");
            }
            catch (Exception e)
            {
                response.StatusCode = 500;
                response.StatusDescription = "Interal Server Error";
                buffer = Encoding.UTF8.GetBytes($"NOK - {e}");
            }

            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        public Task HandleIncomingConnections()
        {
            var task = new Task(HandleSync);
            task.Start();
            return task;
        }
    }
}