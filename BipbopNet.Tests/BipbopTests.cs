using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BipbopNet.Tests
{
    public class BipbopTests
    {
        private readonly Lazy<Client> _juristekClient = new Lazy<Client>(() =>
            new Client(Environment.GetEnvironmentVariable("BIPBOP_APIKEY")));

        public Client Client => _juristekClient.Value;

        [Test]
        public async Task RFB()
        {
            var description = await Client.Description;
            var rfbDatabase = description.Databases.First(database =>
                string.Compare(database.Name, "RFB", StringComparison.OrdinalIgnoreCase) == 0);
            var rfbTable = rfbDatabase.Tables.First(table =>
                string.Compare(table.Name, "CERTIDAO", StringComparison.OrdinalIgnoreCase) == 0);
            Assert.IsNotNull(rfbTable);
            Assert.IsNotNull(rfbDatabase);
            var query = await Client.Request("SELECT FROM 'RFB'.'CERTIDAO'", new[]
            {
                new KeyValuePair<string, string>("DOCUMENTO", "375.543.118-16"),
                new KeyValuePair<string, string>("NASCIMENTO", "08/06/1990"),
            }); 
        }
    }
}