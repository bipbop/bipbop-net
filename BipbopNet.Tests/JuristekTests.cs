#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BipbopNet.Juristek;
using BipbopNet.Parser;
using BipbopNet.Push;
using NUnit.Framework;

namespace BipbopNet.Tests
{
    public class JuristekTests
    {
        private readonly Lazy<Juristek.Client> _juristekClient = new Lazy<Juristek.Client>(() =>
            new Juristek.Client(new Client(Environment.GetEnvironmentVariable("BIPBOP_APIKEY"))));

        public Juristek.Client JuristekClient => _juristekClient.Value;

        [Test]
        public async Task PushListTest()
        {
            var list = await JuristekClient.Push.List(new ListParameters());
            foreach (var i in list) Console.WriteLine(list);
        }

        [Test]
        public async Task OabTest()
        {
            var oab = await JuristekClient.OABProcesso("60438-PR", new Uri(await Listener.ServerAddr()));
            foreach (var oabPush in oab.Pushes) Assert.IsNotNull(await JuristekClient.Push.Status(oabPush));

            Assert.IsNotEmpty(oab.Advogados.ToList());
        }

        [Test]
        public async Task HttpServer()
        {
            var listener = new Listener();
            var cnjQuery = Query.Cnj("0016306-32.2019.8.26.0502");
            var pushConfiguration = await JuristekClient.Push.Create(JuristekClient.CreatePushConfiguration(cnjQuery,
                new PushConfiguration
                {
                    MaxVersion = 1,
                    Expire = DateTime.Now.AddMinutes(30),
                    Callback = await Listener.ServerAddr()
                }));

            listener.OnRequest += (sender, args) =>
            {
                if (args.Push.Id != pushConfiguration.Id) return;
                listener.Stop();
            };

            await listener.HandleIncomingConnections();
        }

        [Test]
        public void GenerateCnjQuery()
        {
            var cnjQuery = Query.Cnj("0016306-32.2019.8.26.0502");
            Assert.AreEqual(cnjQuery.ToString(),
                "SELECT FROM 'CNJ'.'PROCESSO' WHERE 'NUMERO_PROCESSO' = '0016306-32.2019.8.26.0502' AND 'UPLOAD' = 'FALSE'");
        }

        [Test]
        public async Task PushTest()
        {
            var cnjQuery = Query.Cnj("0016306-32.2019.8.26.0502");
            var webSocket = new WebSocket(JuristekClient.BipbopClient);
            await webSocket.Start();

            var pushConfiguration = await JuristekClient.Push.Create(JuristekClient.CreatePushConfiguration(cnjQuery,
                new PushConfiguration
                {
                    MaxVersion = 1,
                    At = DateTime.Now.AddMinutes(1), /* Tempo para conectar o WebSocket */
                    Expire = DateTime.Now.AddMinutes(30)
                }));
            Assert.IsNotEmpty(pushConfiguration.Id);
            var pushStatus = await JuristekClient.Push.Status(pushConfiguration);

            Assert.AreEqual(pushStatus.Push.Id, pushConfiguration.Id);
            Assert.IsNotEmpty(pushStatus.Push.Label);
            var document = await webSocket.WaitPush(pushStatus.Push);
            Assert.IsNotNull(document);
            Assert.IsNotEmpty(new Processos(document.Document).Retrieve);
            var pushDocument = await JuristekClient.Push.Document(pushStatus.Push);
            Assert.IsNotNull(pushDocument);
            Assert.IsNotEmpty(new Processos(pushDocument.Document).Retrieve);
        }

        [Test]
        public async Task PushDelete()
        {
            var cnjQuery = Query.Cnj("0016306-32.2019.8.26.0502");
            var pushConfiguration = await JuristekClient.Push.Create(JuristekClient.CreatePushConfiguration(cnjQuery,
                new PushConfiguration
                {
                    MaxVersion = 1,
                    At = DateTime.Now.AddMinutes(1), /* Tempo para conectar o WebSocket */
                    Expire = DateTime.Now.AddMinutes(30),
                    Parameters = new List<KeyValuePair<string, string>>
                    {
                        KeyValuePair.Create("pushLocked", "true")
                    }
                }));
            Assert.IsNotEmpty(pushConfiguration.Id);
            var pushStatus = await JuristekClient.Push.Status(pushConfiguration);
            Assert.AreEqual(pushStatus.Push.Id, pushConfiguration.Id);
            await JuristekClient.Push.Delete(pushConfiguration);
        }


        [Test]
        public async Task RealtimeQuery()
        {
            var description = await JuristekClient.Description;
            var tjspDatabase = description.Databases.First(database =>
                string.Compare(database.Name, "TJSP", StringComparison.OrdinalIgnoreCase) == 0);
            var tjspTable = tjspDatabase.Tables.First(table =>
                string.Compare(table.Name, "PrimeiraInstancia", StringComparison.OrdinalIgnoreCase) == 0);

            Assert.Throws<QueryException>(() => new Query(tjspTable));

            var tjspQuery = new Query(tjspTable, new[]
            {
                KeyValuePair.Create("NUMERO_PROCESSO", "0016306-32.2019.8.26.0502")
            });

            var bipbopDocument = await JuristekClient.Request(tjspQuery);
            var processos = new Processos(bipbopDocument.Document);
            Assert.IsNotEmpty(processos.Retrieve);
            var bipbopDocumentRepeat = await JuristekClient.Request(Query.Cnj(processos.Retrieve.First()));
            Assert.IsNotEmpty(new Processos(bipbopDocumentRepeat.Document).Retrieve);
        }
    }
}