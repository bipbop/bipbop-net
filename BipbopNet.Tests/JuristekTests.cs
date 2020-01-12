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
            new Juristek.Client(new Client()));

        public Juristek.Client JuristekClient => _juristekClient.Value;

        [Test]
        public async Task PushListTest()
        {
            var listParameters = new ListParameters();
            var list = await JuristekClient.Push.List(listParameters);
            foreach (var i in list.Items) Console.WriteLine(i);
        }

        [Test]
        public async Task OabTest()
        {
            var uniq = Guid.NewGuid().ToString("N");

            var oabParameters = new OABParameters("312375")
            {
                Estado = Juristek.Client.Estado.SP,
                OrigemOab = Juristek.Client.Estado.SP,
                TipoInscricao = TipoInscricao.Advogado,
                WebSocket = true,
                Marker = uniq,
                Label = uniq
            };

            var oab = await JuristekClient.OabProcesso(oabParameters);
            await JuristekClient.WebSocket.Start();


            var documents = JuristekClient.WebSocket.WaitPush(oab, timeout: 60);
            var progress = new Progress(JuristekClient.Push, uniq);
            Console.WriteLine(await progress.calculate());
        }

        [Test]
        public async Task HttpServer()
        {
            var listener = new Listener();
            await listener.Start();

            var cnjQuery = Query.Cnj("0016306-32.2019.8.26.0502");
            var pushConfiguration = await JuristekClient.Push.Create(Juristek.Client.CreatePushConfiguration(cnjQuery,
                new Configuration
                {
                    MaxVersion = 1,
                    Expire = DateTime.Now.AddMinutes(30),
                    Callback = await listener.ServerAddr
                }));

            listener.OnRequest += (sender, args) =>
            {
                if (args.Job.Id != pushConfiguration.Id) return;
                listener.Stop();
            };

            await listener.HandleConnectionsAsync();
            listener.StopSync();
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
            var webSocket = JuristekClient.BipbopClient.WebSocket;
            await webSocket.Start();

            var pushConfiguration = await JuristekClient.Push.Create(Juristek.Client.CreatePushConfiguration(cnjQuery,
                new Configuration
                {
                    MaxVersion = 1,
                    At = DateTime.Now.AddMinutes(1), /* Tempo para conectar o WebSocket */
                    Expire = DateTime.Now.AddMinutes(30)
                }));
            Assert.IsNotEmpty(pushConfiguration.Id);
            var pushStatus = await JuristekClient.Push.Status(pushConfiguration);

            Assert.AreEqual(pushStatus.Job.Id, pushConfiguration.Id);
            Assert.IsNotEmpty(pushStatus.Job.Label);
            var document = await webSocket.WaitPush(pushStatus.Job);
            Assert.IsNotNull(document);
            Assert.IsNotEmpty(new Processos(document.Response.Document).Retrieve);
            var pushDocument = await JuristekClient.Push.Document(pushStatus.Job);
            Assert.IsNotNull(pushDocument);
            Assert.IsNotEmpty(new Processos(pushDocument.Document).Retrieve);
        }

        [Test]
        public async Task PushDelete()
        {
            var cnjQuery = Query.Cnj("0016306-32.2019.8.26.0502");
            var pushConfiguration = await JuristekClient.Push.Create(Juristek.Client.CreatePushConfiguration(cnjQuery,
                new Configuration
                {
                    MaxVersion = 1,
                    At = DateTime.Now.AddMinutes(1), /* Tempo para conectar o WebSocket */
                    Expire = DateTime.Now.AddMinutes(30),
                    Parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("pushLocked", "true")
                    }
                }));
            Assert.IsNotEmpty(pushConfiguration.Id);
            var pushStatus = await JuristekClient.Push.Status(pushConfiguration);
            Assert.AreEqual(pushStatus.Job.Id, pushConfiguration.Id);
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

            Assert.Throws<QueryException>(() =>
            {
                var query = new Query(tjspTable);
                Console.WriteLine(query.ToString());
            });

            var tjspQuery = new Query(tjspTable, new[]
            {
                new KeyValuePair<string, string>("NUMERO_PROCESSO", "0016306-32.2019.8.26.0502")
            });

            var bipbopDocument = await JuristekClient.Request(tjspQuery);
            var processos = new Processos(bipbopDocument.Document);
            Assert.IsNotEmpty(processos.Retrieve);
            var processo = processos.Retrieve.First();
            var bipbopDocumentRepeat = await JuristekClient.Request(Query.Cnj(processo));
            Assert.IsNotEmpty(new Processos(bipbopDocumentRepeat.Document).Retrieve);
        }
    }
}