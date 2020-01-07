using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BipbopNet.Push
{
    using Handler = Func<HttpListenerContext, HttpListenerResponse, byte[]>;

    /// <summary>
    /// Escuta por Trabalhos da BIPBOP
    /// </summary>
    public class Listener
    {
        private static readonly Lazy<Task<string>> LazyIpResponse = new Lazy<Task<string>>(IpResponse);
        private static readonly Random _random = new Random();
        private readonly Mutex _handleMutex = new Mutex();
        private readonly HttpListener _httpListener;
        private readonly string _token = _generateToken();

        /// <summary>
        /// Porta onde o servidor HTTP escuta
        /// </summary>
        public readonly int Port;

        private bool _runServer;

        /// <summary>
        /// Escuta por Trabalhos da BIPBOP
        /// </summary>
        public Listener(int? userPort = null)
        {
            _httpListener = new HttpListener();
            Port = userPort ?? DefaultPort;
            _httpListener.Prefixes.Add($"http://*:{Port}/");
        }

        private static int DefaultPort => EnvironmentPort();

        /// <summary>
        /// IP do Servidor na Internet
        /// </summary>
        public static Task<string> Ip => LazyIpResponse.Value;

        /// <summary>
        /// Endereço do Servidor na Internet (terá de ser feito NAT)
        /// </summary>
        /// <value>Endereço do Servidor</value>
        public Task<string> ServerAddr => GenerateServerAddr(Port);

        private static string _generateToken(int length = 64)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Inicia o Servidor
        /// </summary>
        public async Task Start(bool validateServerAddr = true)
        {
            _runServer = true;
            _httpListener.Start();

            byte[] TemporaryHandler(HttpListenerContext ctx, HttpListenerResponse res)
            {
                res.StatusCode = 503;
                return Encoding.UTF8.GetBytes(_token);
            }

            var stop = false;
            var handle = HandleConnectionsAsync(TemporaryHandler, () => stop);
            try
            {
                await TokenRequest();
            }
            finally
            {
                stop = true;
            }

            await handle;
        }

        private static int EnvironmentPort()
        {
            var environmentPort = Environment.GetEnvironmentVariable("BIPBOP_SERVER_PORT");
            if (string.IsNullOrEmpty(environmentPort)) return 8081;
            var port = int.Parse(environmentPort);
            if (port < 0 || port > 65535) return 8081;
            return port;
        }

        /// <summary>
        /// Escuta por Eventos
        /// </summary>
        /// <code>
        /// var listener = new Listener();
        /// listener.OnRequest += (context, listenerEvent) => {};
        /// await listener.HandleIncomingConnections();
        /// </code>
        public event EventHandler<ListenerEvent> OnRequest;


        /// <summary>
        /// Endereço do Servidor
        /// </summary>
        /// <param name="port">Porta do Servidor</param>
        /// <returns></returns>
        public static async Task<string> GenerateServerAddr(int? port = null)
        {
            var envData = Environment.GetEnvironmentVariable("BIPBOP_SERVER");
            if (envData != null) return envData;
            var ipResponse = await Ip;
            return $"http://{ipResponse}:{port ?? DefaultPort}/";
        }

        private async Task TokenRequest()
        {
            var ipRequest = await new HttpClient().GetAsync(await ServerAddr);
            var ipResponse = (await ipRequest.Content.ReadAsStringAsync()).Trim();
            if (ipResponse != _token) throw new HttpListenerException();
        }

        /// <summary>
        /// Resposta do IP
        /// </summary>
        /// <returns></returns>
        private static async Task<string> IpResponse()
        {
            var ipRequest = await new HttpClient().GetAsync("http://ipinfo.io/ip");
            var ipResponse = (await ipRequest.Content.ReadAsStringAsync()).Trim();
            return ipResponse;
        }

        /// <summary>
        /// Para o Servidor Assíncrono
        /// </summary>
        /// <returns>Promessa</returns>
        public Task Stop()
        {
            return Task.Run(() => StopSync());
        }

        /// <summary>
        /// Para o Servidor
        /// </summary>
        public void StopSync()
        {
            _runServer = false;
            _handleMutex.WaitOne();
            _httpListener.Stop();
            _handleMutex.ReleaseMutex();
        }

        ~Listener()
        {
            StopSync();
            _handleMutex.Dispose();
        }

        /// <summary>
        /// Captura as Conexões do HTTP para Gerar Eventos
        /// </summary>
        public void HandleConnections()
        {
            HandleConnections(null);
        }

        private void HandleConnections(Handler handler = null, Func<bool> stopCondition = null)
        {
            _handleMutex.WaitOne();
            while ((stopCondition?.Invoke() ?? _runServer) && _httpListener.IsListening)
            {
                HandleConnection(handler);
            }

            _handleMutex.ReleaseMutex();
        }

        private void HandleConnection(Handler userHandler = null)
        {
            var handler = userHandler ?? ConnectionBuffer;
            var context = _httpListener.GetContext();
            var response = context.Response;
            response.ContentType = "plain/text";
            var buffer = handler(context, response);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private byte[] ConnectionBuffer(HttpListenerContext context, HttpListenerResponse response)
        {
            byte[] buffer;
            try
            {
                OnRequest?.Invoke(context, new ListenerEvent(context.Request));
                buffer = Encoding.UTF8.GetBytes("OK");
            }
            catch (Exception e)
            {
                response.StatusCode = 500;
                response.StatusDescription = "Interal Server Error";
                buffer = Encoding.UTF8.GetBytes($"NOK - {e}");
            }

            return buffer;
        }

        /// <summary>
        /// Captura as conexões do HTTP para gerar eventos
        /// </summary>
        /// <returns>Promessa</returns>
        public Task HandleConnectionsAsync()
        {
            return HandleConnectionsAsync(null);
        }

        private Task HandleConnectionsAsync(Handler userHandler = null, Func<bool> stopCondition = null)
        {
            return Task.Run(() => HandleConnections(userHandler, stopCondition));
        }
    }
}