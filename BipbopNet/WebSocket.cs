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
    public class WebSocket
    {
        private const string DefaultUrl = "wss://irql.bipbop.com.br/ws";

        public readonly WebsocketClient Client;
        public readonly Client WebServiceClient;
        
        public WebSocket(Client client)
        {
            WebServiceClient = client;
            var timeout = Environment.GetEnvironmentVariable("BIPBOP_WEBSOCKET_RECONNECT") ?? "30";
            var url = Environment.GetEnvironmentVariable("BIPBOP_WEBSOCKET") ?? DefaultUrl;
            var uri = new Uri(url);
            Client = new WebsocketClient(uri,
                new Func<ClientWebSocket>(() => new ClientWebSocket {Options = {Proxy = client.Proxy,}}))
            {
                ReconnectTimeout = TimeSpan.FromSeconds(int.Parse(timeout))
            };
        }

        public async Task Start()
        {
            await Client.Start();
            Client.ReconnectionHappened.Subscribe(info => Client.Send(JsonConvert.SerializeObject(WebServiceClient.ApiKey)));
            await Client.SendInstant(JsonConvert.SerializeObject(WebServiceClient.ApiKey));
        }

        
        
        public async Task<BipbopDocument?> WaitPush(PushIdentifier? pushIdentifier)
        {
            if (pushIdentifier == null) return null;
            var stopEvent = new ManualResetEvent(false);
            XmlDocument document = new XmlDocument();
            var messageSubscription = Client.MessageReceived.Subscribe((info) =>
            {
                var method = JObject.Parse(info.Text);
                if (method["method"]?.ToString() != "pushUpdate") return;
                if (pushIdentifier.Label != null &&
                    method["data"]?["pushObject"]?["label"]?.ToString() != pushIdentifier.Label) return;
                if (pushIdentifier.Id != null &&
                    method["data"]?["pushObject"]?["_id"]?.ToString() != pushIdentifier.Id) return;
                if (method["data"]?["document"]?["data"] == null) return; 
                document.LoadXml(method["data"]?["document"]?["data"]?.ToString());
                stopEvent.Set();
            });
            
            stopEvent.WaitOne();
            messageSubscription.Dispose();
            return new BipbopDocument(document);
        }
    }
}