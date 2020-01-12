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
    ///     Escuta por Trabalhos da BIPBOP
    /// </summary>
    public class Listener
    {
        private static readonly Lazy<Task<string>> LazyIpResponse = new Lazy<Task<string>>(() => IpResponse());
        private static readonly Random _random = new Random();
        private readonly Mutex _handleMutex = new Mutex();
        private readonly HttpListener _httpListener;
        private readonly string _token = _generateToken();

        /// <summary>
        ///     Porta onde o servidor HTTP escuta
        /// </summary>
        public readonly int Port;

        private bool _runServer;

        /// <summary>
        ///     Escuta por Trabalhos da BIPBOP
        /// </summary>
        public Listener(int? userPort = null)
        {
            _httpListener = new HttpListener();
            Port = userPort ?? DefaultPort;
            _httpListener.Prefixes.Add($"http://*:{Port}/");
        }

        private static int DefaultPort => EnvironmentPort();

        /// <summary>
        ///     IP do Servidor na Internet
        /// </summary>
        public static Task<string> Ip => LazyIpResponse.Value;

        /// <summary>
        ///     Endereço do Servidor na Internet (terá de ser feito NAT)
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
        ///     Inicia o Servidor
        /// </summary>
        public async Task Start(bool validateServerAddr = true, int timeout = 30)
        {
            _runServer = true;
            _httpListener.Start();
            if (!validateServerAddr) return;

            byte[] TemporaryHandler(HttpListenerContext ctx, HttpListenerResponse res)
            {
                res.StatusCode = 503;
                return Encoding.UTF8.GetBytes(_token);
            }

            var canContinue = true;
            var exitEvent = new ManualResetEvent(false);

            // ReSharper disable once AccessToModifiedClosure
            var handle = HandleConnectionsAsync(TemporaryHandler, () => canContinue, exitEvent);
            try
            {
                await TokenRequest(timeout);
            }
            finally
            {
                canContinue = false;
                exitEvent.Set();
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
        ///     Escuta por Eventos
        /// </summary>
        /// <code>
        /// var listener = new Listener();
        /// listener.OnRequest += (context, listenerEvent) => {};
        /// await listener.HandleIncomingConnections();
        /// </code>
        public event EventHandler<ListenerEvent> OnRequest;


        /// <summary>
        ///     Endereço do Servidor
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

        private async Task TokenRequest(int timeout = 30)
        {
            var httpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(timeout)};
            var tokenResponse = await httpClient.GetAsync(await ServerAddr);
            var tokenData = (await tokenResponse.Content.ReadAsStringAsync()).Trim();
            if (tokenData != _token) throw new ListenerException();
        }

        /// <summary>
        ///     Resposta do IP
        /// </summary>
        /// <returns></returns>
        private static async Task<string> IpResponse(int timeout = 30)
        {
            var httpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(timeout)};
            var ipRequest = await httpClient.GetAsync("http://ipinfo.io/ip");
            var ipResponse = (await ipRequest.Content.ReadAsStringAsync()).Trim();
            return ipResponse;
        }

        /// <summary>
        ///     Para o Servidor Assíncrono
        /// </summary>
        /// <returns>Promessa</returns>
        public Task Stop()
        {
            return Task.Run(() => StopSync());
        }

        /// <summary>
        ///     Para o Servidor
        /// </summary>
        public void StopSync()
        {
            if (!_runServer) return;
            _runServer = false;
            if (!_httpListener.IsListening) return;
            _handleMutex.WaitOne();
            try
            {
                _httpListener.Stop();
            }
            finally
            {
                _handleMutex.ReleaseMutex();
            }
        }

        ~Listener()
        {
            StopSync();
            _handleMutex.Dispose();
        }

        /// <summary>
        ///     Captura as Conexões do HTTP para Gerar Eventos
        /// </summary>
        public void HandleConnections()
        {
            HandleConnections(null);
        }

        private void HandleConnections(Handler handler = null, Func<bool> continueCallback = null,
            ManualResetEvent reset = null)
        {
            _handleMutex.WaitOne();
            try
            {
                while ((continueCallback?.Invoke() ?? _runServer) && _httpListener.IsListening)
                {
                    HandleConnection(handler);
                    reset?.WaitOne();
                }
            }
            finally
            {
                _handleMutex.ReleaseMutex();
            }
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
        ///     Captura as conexões do HTTP para gerar eventos
        /// </summary>
        /// <returns>Promessa</returns>
        public Task HandleConnectionsAsync()
        {
            return HandleConnectionsAsync(null);
        }

        private Task HandleConnectionsAsync(Handler userHandler = null, Func<bool> continueCallback = null,
            ManualResetEvent reset = null)
        {
            return Task.Run(() => HandleConnections(userHandler, continueCallback, reset));
        }
    }

    internal class ListenerException : Exception
    {
    }
}