using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using BipbopNet.Parser;
using BipbopNet.Push;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Websocket.Client;
using Exception = BipbopNet.Parser.Exception;

namespace BipbopNet
{
    using HookHandler = Action<JobIdentifier, BipbopResponse>;

    /// <summary>
    ///     Realiza as chamadas ao WebService da BIPBOP
    /// </summary>
    public class Client
    {
        private const string ApiKeyParameter = "apiKey";
        private const string QueryParameter = "q";
        private const string Endpoint = "https://irql.bipbop.com.br/";
        private readonly Uri _endPoint;
        private readonly Lazy<Task<Info>> _lazyDescription;
        private readonly Lazy<WebSocket> _lazyWebSocket;

        /// <summary>
        ///     Chave de API da BIPBOP
        /// </summary>
        public readonly string ApiKey;

        /// <summary>
        ///     Proxy que a BIPBOP utiliza para suas requisições.
        /// </summary>
        public readonly WebProxy Proxy;

        /// <summary>
        ///     Inicializa um Cliente BIPBOP
        /// </summary>
        /// <param name="apiKey">Chave de API da BIPBOP</param>
        /// <param name="endpoint">Endpoint da BIPBOP</param>
        /// <param name="proxy">Proxy</param>
        public Client(string apiKey = null, string endpoint = null, string proxy = null)
        {
            var proxyUrl = proxy ?? Environment.GetEnvironmentVariable("BIPBOP_PROXY");
            ApiKey = apiKey ?? Environment.GetEnvironmentVariable("BIPBOP_APIKEY");
            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(ApiKey));
            Proxy = proxyUrl != null ? new WebProxy(new Uri(proxyUrl)) : null;
            _endPoint = new Uri(endpoint ?? Environment.GetEnvironmentVariable("BIPBOP_ENDPOINT") ?? Endpoint);
            _lazyDescription = new Lazy<Task<Info>>(RequestDescription);
            _lazyWebSocket = new Lazy<WebSocket>(() => new WebSocket(this));
        }


        /// <summary>
        ///     Captura um WebSocket
        /// </summary>
        public WebSocket WebSocket => _lazyWebSocket.Value;

        /// <summary>
        ///     Descrição das Consultas Disponíveis
        /// </summary>
        /// <code>
        /// var description = await Client.Description;
        /// var rfbDatabase = description.Databases.First(database => string.Compare(database.Name, "RFB", StringComparison.OrdinalIgnoreCase) == 0);
        /// var rfbTable = rfbDatabase.Tables.First(table => string.Compare(table.Name, "CERTIDAO", StringComparison.OrdinalIgnoreCase) == 0);
        /// </code>
        public Task<Info> Description => _lazyDescription.Value;

        private async Task<Info> RequestDescription()
        {
            var description = await Request("SELECT FROM 'INFO'.'INFO'");
            return new Info(description.Document);
        }

        /// <summary>
        ///     Requisição
        /// </summary>
        /// <param name="query">Consulta</param>
        /// <param name="clientParameters">Parâmetros de Consulta</param>
        /// <returns>Documento</returns>
        public Task<BipbopDocument> Request(Table query,
            IEnumerable<KeyValuePair<string, string>> clientParameters = null)
        {
            return Request(query.SelectString(), clientParameters);
        }

        /// <summary>
        ///     Requisição
        /// </summary>
        /// <param name="query">Consulta</param>
        /// <param name="clientParameters">Parâmetros de Consulta</param>
        /// <returns>Documento</returns>
        public async Task<BipbopDocument> Request(string query,
            IEnumerable<KeyValuePair<string, string>> clientParameters = null)
        {
            var request = await Response(query, clientParameters);
            var response = await request.Content.ReadAsStringAsync();
            var document = new XmlDocument();
            document.LoadXml(response);
            return new BipbopDocument(document);
        }

        /// <summary>
        ///     Requisição especial para chamadas JSON
        /// </summary>
        /// <param name="query">Consulta</param>
        /// <param name="clientParameters">Parâmetros de Consulta</param>
        /// <returns>Documento</returns>
        public Task<JObject> JRequest(Table query,
            IEnumerable<KeyValuePair<string, string>> clientParameters = null)
        {
            return JRequest(query.SelectString(), clientParameters);
        }


        /// <summary>
        ///     Requisição especial para chamadas JSON
        /// </summary>
        /// <param name="query">Consulta</param>
        /// <param name="clientParameters">Parâmetros de Consulta</param>
        /// <returns>JObject para você analisar o retorno</returns>
        /// <exception cref="Parser.Exception">
        ///     Caso você chame um documento que retorna XML e não JSON ou exceção durante a
        ///     consulta
        /// </exception>
        public async Task<JObject> JRequest(string query,
            IEnumerable<KeyValuePair<string, string>> clientParameters = null)
        {
            var request = await Response(query, clientParameters);
            var response = await request.Content.ReadAsStringAsync();
            if (request.Content?.Headers?.ContentType?.MediaType == "application/json") return JObject.Parse(response);
            var document = new XmlDocument();
            document.LoadXml(response);
            var unused = new BipbopDocument(document); /* assertion */
            throw new Exception("Unexpected Content (Type was returned)");
        }


        private async Task<HttpResponseMessage> Response(string query,
            IEnumerable<KeyValuePair<string, string>> clientParameters)
        {
            var httpClient = new HttpClient(new HttpClientHandler
            {
                Proxy = Proxy
            })
            {
                Timeout = TimeSpan.FromMinutes(5) /* tempo que no máximo os servidores da BIPBOP respondem */
            };

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(ApiKeyParameter, ApiKey),
                new KeyValuePair<string, string>(QueryParameter, query)
            };

            var formParameters =
                new FormUrlEncodedContent(clientParameters != null ? parameters.Concat(clientParameters) : parameters);
            return await httpClient.PostAsync(_endPoint, formParameters);
        }
    }

    /// <summary>
    ///     Hook via WebSocket da BIPBOP
    /// </summary>
    public class WebSocket
    {
        private const string DefaultUrl = "wss://irql.bipbop.com.br/ws";

        private readonly WebsocketClient _client;

        /// <summary>
        ///     Endpoint do WebSocket
        /// </summary>
        public readonly Uri Endpoint;

        /// <summary>
        ///     Timeout para haver reconexão
        /// </summary>
        public readonly int Timeout;

        /// <summary>
        ///     Cliente da API BIPBOP
        /// </summary>
        public readonly Client WebServiceClient;

        /// <summary>
        ///     Hook via WebSocket, reconexão automática
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
        internal WebSocket(Client client = null, Uri endpoint = null, int? timeout = null)
        {
            WebServiceClient = client ?? new Client();
            var timeoutStr = Environment.GetEnvironmentVariable("BIPBOP_WEBSOCKET_RECONNECT") ?? "30";
            Timeout = timeout ?? int.Parse(timeoutStr);
            Endpoint = endpoint ?? new Uri(Environment.GetEnvironmentVariable("BIPBOP_WEBSOCKET") ?? DefaultUrl);
            _client = new WebsocketClient(Endpoint,
                () => new ClientWebSocket {Options = {Proxy = client?.Proxy}})
            {
                ReconnectTimeout = TimeSpan.FromSeconds(Timeout)
            };
            _client.ReconnectionHappened.Subscribe(info =>
                _client.Send(JsonConvert.SerializeObject(WebServiceClient.ApiKey)));
        }

        private event EventHandler OnStop;


        ~WebSocket()
        {
            _client.Stop(WebSocketCloseStatus.NormalClosure, "");
        }

        /// <summary>
        ///     Para a conexão
        /// </summary>
        /// <returns>Tarefa de Parar</returns>
        public async Task Stop()
        {
            OnStop?.Invoke(null, null);
            await _client.Stop(0, null);
        }

        /// <summary>
        ///     Inicia a conexão
        /// </summary>
        /// <returns>Tarefa de Iniciar Conexão</returns>
        public async Task Start()
        {
            if (_client.IsRunning) return;
            await _client.Start();
            await _client.SendInstant(JsonConvert.SerializeObject(WebServiceClient.ApiKey));
        }


        /// <summary>
        ///     Aguarda um PUSH ser concluído.
        /// </summary>
        /// <param name="jobIdentifier">Identificador do Push</param>
        /// <param name="timeout">Tempo máximo de execução no sistema</param>
        /// <returns>Documento BIPBOP</returns>
        public Task<BipbopResponse> WaitPush(JobIdentifier jobIdentifier, int timeout = 0)
        {
            return Task.Run(() => WaitPushSync(jobIdentifier, timeout));
        }

        /// <summary>
        ///     Aguarda pelos PUSHs da OAB
        /// </summary>
        /// <param name="pushIdentifiers">Lista de PUSH OAB</param>
        /// <param name="handler">Captura no Intervalo</param>
        /// <param name="timeout">Timeout</param>
        /// <returns>Documentos de Consultas que Finalizaram</returns>
        public IEnumerable<KeyValuePair<JobIdentifier, BipbopResponse>> WaitPush(
            OabConsulta pushIdentifiers, HookHandler handler = null, int timeout = 0)
        {
            return WaitPush(pushIdentifiers.Pushes, handler, timeout);
        }

        /// <summary>
        ///     Aguarda múltiplos PUSH
        /// </summary>
        /// <param name="pushIdentifiers">Push</param>
        /// <param name="handler">Captura durante o processamento de todos</param>
        /// <param name="timeout">timeout</param>
        /// <returns>Documentos de Consultas que Finalizaram</returns>
        public IEnumerable<KeyValuePair<JobIdentifier, BipbopResponse>> WaitPush(
            IEnumerable<JobIdentifier> pushIdentifiers, HookHandler handler = null, int timeout = 0)
        {
            var tasksWaitingPush = pushIdentifiers
                .Select(pushIdentifier => Task.Run(() =>
                {
                    var response = WaitPushSync(pushIdentifier, timeout);
                    handler?.Invoke(pushIdentifier, response);
                    return new KeyValuePair<JobIdentifier, BipbopResponse>(pushIdentifier, response);
                })).ToArray();

            if (timeout > 0) Task.WaitAll(tasksWaitingPush, TimeSpan.FromSeconds(timeout));
            else Task.WaitAll(tasksWaitingPush);

            return tasksWaitingPush.Where(task => task.IsCompleted).Select(task => task.Result);
        }

        /// <summary>
        ///     Aguarda por um processo
        /// </summary>
        /// <param name="jobIdentifier">Push</param>
        /// <param name="timeout">ms que irá esperar</param>
        /// <returns>Documento</returns>
        public BipbopResponse WaitPushSync(JobIdentifier jobIdentifier, int timeout = 0)
        {
            if (!_client.IsRunning) throw new Exception("WebSocket not stated");
            if (jobIdentifier == null) return null;

            var tokenSource = new CancellationTokenSource();
            var exitEvent = new ManualResetEvent(false);
            XmlDocument document = null;

            void StopEvent(object sender, EventArgs args)
            {
                exitEvent.Set();
            }

            OnStop += StopEvent;


            if (timeout > 0)
                Task
                    .Delay(TimeSpan.FromSeconds(timeout), tokenSource.Token)
                    .ContinueWith(timeoutTask => exitEvent.Set(), tokenSource.Token);

            var messageSubscription = _client.MessageReceived.Subscribe(info =>
            {
                OnStop -= StopEvent;
                var method = JObject.Parse(info.Text);
                if (method["method"]?.ToString() != "pushUpdate") return;
                if (jobIdentifier.Label != null &&
                    method["data"]?["pushObject"]?["label"]?.ToString() != jobIdentifier.Label) return;
                if (jobIdentifier.Id != null &&
                    method["data"]?["pushObject"]?["_id"]?.ToString() != jobIdentifier.Id) return;
                if (method["data"]?["document"]?["data"] == null) return;
                document = new XmlDocument();
                document.LoadXml(method["data"]?["document"]?["data"]?.ToString());
                exitEvent.Set();
            });

            exitEvent.WaitOne();
            OnStop -= StopEvent;
            tokenSource.Cancel();
            messageSubscription.Dispose();
            return document == null ? null : BipbopResponse.loadDocument(document);
        }
    }
}