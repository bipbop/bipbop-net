using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using BipbopNet.Parser;
using Newtonsoft.Json.Linq;
using Exception = BipbopNet.Parser.Exception;

namespace BipbopNet
{
    
    /// <summary>
    /// Realiza as chamadas ao WebService da BIPBOP
    /// </summary>
    public class Client
    {
        private const string ApiKeyParameter = "apiKey";
        private const string QueryParameter = "q";
        private const string Endpoint = "https://irql.bipbop.com.br/";
        private readonly Uri _endPoint;
        private readonly Lazy<Task<Info>> _lazyDescription;

        /// <summary>
        /// Chave de API da BIPBOP
        /// </summary>
        public readonly string ApiKey;

        /// <summary>
        /// Proxy que a BIPBOP utiliza para suas requisições.
        /// </summary>
        public readonly WebProxy Proxy;

        /// <summary>
        /// Inicializa um Cliente BIPBOP
        /// </summary>
        /// <param name="apiKey">Chave de API da BIPBOP</param>
        /// <param name="endpoint">Endpoint da BIPBOP</param>
        /// <param name="proxy">Proxy</param>
        public Client(string apiKey = null, string endpoint = null, string proxy = null)
        {
            var proxyUrl = proxy ?? Environment.GetEnvironmentVariable("BIPBOP_PROXY");
            ApiKey = apiKey ?? Environment.GetEnvironmentVariable("BIPBOP_APIKEY");
            Proxy = proxyUrl != null ? new WebProxy(new Uri(proxyUrl)) : null;
            _endPoint = new Uri(endpoint ?? Environment.GetEnvironmentVariable("BIPBOP_ENDPOINT") ?? Endpoint);
            _lazyDescription = new Lazy<Task<Info>>(RequestDescription);
        }

        /// <summary>
        /// Descrição das Consultas Disponíveis
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
        /// Requisição
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
        /// Requisição
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
        /// Requisição especial para chamadas JSON
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
        /// Requisição especial para chamadas JSON
        /// </summary>
        /// <param name="query">Consulta</param>
        /// <param name="clientParameters">Parâmetros de Consulta</param>
        /// <returns>JObject para você analisar o retorno</returns>
        /// <exception cref="Exception">Caso você chame um documento que retorna XML e não JSON ou exceção durante a consulta</exception>
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
            var httpClient = new HttpClient(new HttpClientHandler {Proxy = Proxy});

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
}