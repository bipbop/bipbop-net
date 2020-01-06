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
    public class Client
    {
        private const string ApiKeyParameter = "apiKey";
        private const string QueryParameter = "q";
        private const string DataParameter = "data";
        private const string Endpoint = "https://irql.bipbop.com.br/";
        private const string JuristekQuery = "SELECT FROM 'JURISTEK'.'REQUEST'";

        private readonly string _endPoint;

        public readonly string ApiKey;
        public readonly WebProxy? Proxy;

        public Client(string apiKey)
        {
            var proxyUrl = Environment.GetEnvironmentVariable("BIPBOP_PROXY");

            ApiKey = apiKey;
            Proxy = proxyUrl != null ? new WebProxy(new Uri(proxyUrl)) : null;
            _endPoint = Environment.GetEnvironmentVariable("BIPBOP_ENDPOINT") ?? Endpoint;
        }

        public async Task<BipbopDocument> Request(string query,
            IEnumerable<KeyValuePair<string, string>>? clientParameters = null)
        {
            var request = await Reponse(query, clientParameters);
            var response = await request.Content.ReadAsStringAsync();
            var document = new XmlDocument();
            document.LoadXml(response);
            return new BipbopDocument(document);
        }

        public async Task<JObject> JRequest(string query,
            IEnumerable<KeyValuePair<string, string>>? clientParameters = null)
        {
            var request = await Reponse(query, clientParameters);
            var response = await request.Content.ReadAsStringAsync();
            if (request.Content?.Headers?.ContentType?.MediaType != "application/json")
            {
                var document = new XmlDocument();
                document.LoadXml(response);
                var bipbopDocument = new BipbopDocument(document); /* assertion */
                throw new Exception("Unexpected Content (Type was returned)");
            }

            return JObject.Parse(response);
        }

        private async Task<HttpResponseMessage> Reponse(string query,
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