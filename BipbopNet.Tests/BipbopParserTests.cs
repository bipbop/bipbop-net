using System;
using NUnit.Framework;

namespace BipbopNet.Tests
{
    public class BipbopParserTests : BipbopTestCommon
    {
        [Test]
        public void PushCanBeTrue()
        {
            try
            {
                ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header><exception push=""true"" id=""5e05c54462b0742a04124cdd"" code=""0"" source=""CompanyException"">É necessário uma chave de acesso</exception></header><body/></BPQL>");
            }
            catch (BipbopParserException e)
            {
                Assert.IsTrue(e.Push);
            }
        }

        [Test]
        public void ParseDescription()
        {
            var document = ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header><description>Teste</description></header><body/></BPQL>");
            Assert.AreEqual(document.Description, "Teste");
        }

        [Test]
        public void NullNextAppointment()
        {
            var document = ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header></header><body/></BPQL>");
            Assert.AreEqual(document.NextAppointment, null);
        }

        [Test]
        public void Query()
        {
            var document = ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header><query>SELECT FROM 'INFO'.'INFO'</query></header><body/></BPQL>");
            Assert.AreEqual(document.Query, "SELECT FROM 'INFO'.'INFO'");
        }


        [Test]
        public void NextAppointment()
        {
            var document = ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header><nextAppointment hour=""23"" minute=""10"" /></header><body/></BPQL>");
            Assert.AreEqual(document.NextAppointment.Hour, 23);
            Assert.AreEqual(document.NextAppointment.Minute, 10);
        }

        [Test]
        public void ParseWarning()
        {
            var document = ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header><warning log=""5e05c54462b0742a04124cdb"" id=""5e05c54462b0742a04124cdd"" code=""0"" source=""CompanyException"">Essa é uma exceção de teste</warning></header><body/></BPQL>");
            Assert.AreEqual(document.Warnings.Length, 1);
            Assert.AreEqual(document.Warnings[0].Message, "Essa é uma exceção de teste");
        }

        [Test]
        public void ResourseUse()
        {
            var doc = ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL resourceUse=""10"" specialResourceUse=""15""><header><execution-time>365</execution-time></header><body/></BPQL>");
            Assert.AreEqual(doc.SpecialResourceUse, 15);
            Assert.AreEqual(doc.ResourceUse, 10);
        }

        [Test]
        public void DateTime()
        {
            var dTime = (DateTime) ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header><date-time>27/12/2019 09:14:25</date-time></header><body/></BPQL>").DateTime;
            Assert.IsInstanceOf<DateTime>(dTime);
            Assert.AreEqual(27, dTime.Day);
            Assert.AreEqual(12, dTime.Month);
            Assert.AreEqual(2019, dTime.Year);
            Assert.AreEqual(9, dTime.Hour);
            Assert.AreEqual(14, dTime.Minute);
            Assert.AreEqual(25, dTime.Second);
        }
        
        [Test]
        public void ExecutionTime()
        {
            Assert.AreEqual(ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header><execution-time>365</execution-time></header><body/></BPQL>").ExecutionTime, 365);
            Assert.AreEqual(ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header></header><body/></BPQL>").ExecutionTime, 0);
        }

        [Test]
        public void ParserException()
        {
            try
            {
                ParseDocument(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BPQL><header><exception
    tableName=""TABLE_TESTE""
    tableDescription=""TableTeste""
    tableUrl=""http://pudim.com.br/table""
    databaseName=""TESTE""
    databaseDescription=""Teste""
    databaseUrl=""http://pudim.com.br/database""
    query=""SELECT FROM 'INFO'.'INFO'""
    log=""5e05c54462b0742a04124cdb"" 
    id=""5e05c54462b0742a04124cdd"" 
    code=""0""
 source=""CompanyException"">É necessário uma chave de acesso</exception></header><body/></BPQL>");
            }
            catch (BipbopParserException e)
            {
                Assert.AreEqual(e.Message, "É necessário uma chave de acesso");
                Assert.AreEqual(e.Id, "5e05c54462b0742a04124cdd");
                Assert.AreEqual(e.Log, "5e05c54462b0742a04124cdb");
                Assert.AreEqual(e.Code, 0);
                Assert.AreEqual(e.Origin, "CompanyException");
                Assert.IsFalse(e.Push);
                Assert.NotNull(e.From);
                Assert.NotNull(e.From.Database);
                Assert.AreEqual(e.From.Database.Name, "TESTE");
                Assert.AreEqual(e.From.Database.Description, "Teste");
                Assert.AreEqual(e.From.Database.Url, "http://pudim.com.br/database");
                Assert.AreEqual(e.From.Name, "TABLE_TESTE");
                Assert.AreEqual(e.From.Description, "TableTeste");
                Assert.AreEqual(e.From.Url, "http://pudim.com.br/table");
                Assert.AreEqual(e.Query, "SELECT FROM 'INFO'.'INFO'");
            }
        }
    }
}