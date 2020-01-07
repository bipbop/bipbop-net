using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using BipbopNet.Parser;
using BipbopNet.Push;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Websocket.Client;

namespace BipbopNet
{
    /// <summary>
    /// Hook via WebSocket da BIPBOP
    /// </summary>
    public class WebSocket
    {
        private const string DefaultUrl = "wss://irql.bipbop.com.br/ws";

        private readonly WebsocketClient _client;

        /// <summary>
        /// Cliente da API BIPBOP
        /// </summary>
        public readonly Client WebServiceClient;

        /// <summary>
        /// Endpoint do WebSocket
        /// </summary>
        public readonly Uri Endpoint;

        /// <summary>
        /// Timeout para haver reconexão
        /// </summary>
        public readonly int Timeout;

        private event EventHandler OnStop;


        ~WebSocket()
        {
            _client.Stop(WebSocketCloseStatus.NormalClosure, "");
        }
        
        /// <summary>
        /// Hook via WebSocket, reconexão automática
        /// </summary>
        /// <code>
        /// var client = new Client();
        /// var webSocket = new WebSocket(client);
        /// await webSocket.Start();
        /// await webSocket.Stop();
        /// </code>
        /// <param name="client">Objeto do WebService</param>
        /// <param name="endpoint">Endereço Alternativo</param>
        /// <param name="timeout">Timeout para Reconexão</param>
        public WebSocket(Client client, Uri endpoint = null, int? timeout = null)
        {
            WebServiceClient = client;
            var timeoutStr = Environment.GetEnvironmentVariable("BIPBOP_WEBSOCKET_RECONNECT") ?? "30";
            Timeout = timeout ?? int.Parse(timeoutStr);
            Endpoint = endpoint ?? new Uri(Environment.GetEnvironmentVariable("BIPBOP_WEBSOCKET") ?? DefaultUrl);
            _client = new WebsocketClient(Endpoint,
                () => new ClientWebSocket {Options = {Proxy = client.Proxy}})
            {
                ReconnectTimeout = TimeSpan.FromSeconds(Timeout)
            };
            _client.ReconnectionHappened.Subscribe(info =>
                _client.Send(JsonConvert.SerializeObject(WebServiceClient.ApiKey)));
        }

        /// <summary>
        /// Para a conexão
        /// </summary>
        /// <returns>Tarefa de Parar</returns>
        public async Task Stop()
        {
            OnStop?.Invoke(null, null);
            await _client.Stop(0, null);
        }
        
        /// <summary>
        /// Inicia a conexão
        /// </summary>
        /// <returns>Tarefa de Iniciar Conexão</returns>
        public async Task Start()
        {
            await _client.Start();
            await _client.SendInstant(JsonConvert.SerializeObject(WebServiceClient.ApiKey));
        }


        /// <summary>
        /// Aguarda um PUSH ser concluído.
        /// </summary>
        /// <param name="pushIdentifier"></param>
        /// <param name="timeout">Tempo máximo de execução no sistema</param>
        /// <returns>Documento BIPBOP</returns>
        public Task<BipbopDocument> WaitPush(PushIdentifier pushIdentifier, int timeout = 0)
        {
            return Task.Run(() => WaitPushSync(pushIdentifier, timeout));
        }

        /// <summary>
        /// Aguarda por um processo
        /// </summary>
        /// <param name="pushIdentifier">Push</param>
        /// <param name="timeout">ms que irá esperar</param>
        /// <returns>Documento</returns>
        public BipbopDocument WaitPushSync(PushIdentifier pushIdentifier, int timeout)
        {
            if (pushIdentifier == null) return null;
            var exitEvent = new ManualResetEvent(false);
            var document = new XmlDocument();

            void StopEvent(object sender, EventArgs args) => exitEvent.Set();

            OnStop += StopEvent;

            if (timeout > 0)
            {
                Task.Delay(timeout).ContinueWith((timeoutTask) => exitEvent.Set());
            }

            var messageSubscription = _client.MessageReceived.Subscribe(info =>
            {
                OnStop -= StopEvent;
                var method = JObject.Parse(info.Text);
                if (method["method"]?.ToString() != "pushUpdate") return;
                if (pushIdentifier.Label != null &&
                    method["data"]?["pushObject"]?["label"]?.ToString() != pushIdentifier.Label) return;
                if (pushIdentifier.Id != null &&
                    method["data"]?["pushObject"]?["_id"]?.ToString() != pushIdentifier.Id) return;
                if (method["data"]?["document"]?["data"] == null) return;
                document.LoadXml(method["data"]?["document"]?["data"]?.ToString());
                exitEvent.Set();
            });


            exitEvent.WaitOne();
            OnStop -= StopEvent;
            messageSubscription.Dispose();
            return new BipbopDocument(document);
        }
    }
}