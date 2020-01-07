using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    public class BipbopDocument
    {
        private readonly CultureInfo _cultureInfo = new CultureInfo("pt-BR");
        public readonly XmlDocument Document;

        /**
         * Nó principal
         */
        protected readonly XmlNode Root;

        /// <summary>
        /// Documento BIPBOP
        /// </summary>
        /// <param name="document">XML recebido</param>
        public BipbopDocument(XmlDocument document)
        {
            Document = document;
            Root = document.DocumentElement;
            AssertException();
        }

        /// <summary>
        /// Quantidade de Recursos (CPUs), utilizadas
        /// </summary>
        public int? ResourceUse => ReadIntegerAttribute(Root, "resourceUse");
        
        /// <summary>
        /// Quantidade de Recursos Especiais (GPUs e CPUs Segunda Geração)
        /// </summary>
        public int? SpecialResourceUse => ReadIntegerAttribute(Root, "specialResourceUse");

        /// <summary>
        /// Tempo de Execução no Backend
        /// </summary>
        public int? ExecutionTime
        {
            get
            {
                var executionTime = Root.SelectSingleNode("./header/execution-time")?.InnerText;
                return executionTime != null ? int.Parse(executionTime) : 0;
            }
        }

        /// <summary>
        /// Data e Hora do Documento
        /// </summary>
        public DateTime? DateTime
        {
            get
            {
                var dateTime = Root.SelectSingleNode("./header/date-time");
                if (dateTime == null) return null;
                return System.DateTime.Parse(dateTime.InnerText, _cultureInfo);
            }
        }


        /// <summary>
        /// Próximo Apontamento
        /// </summary>
        public NextAppointment NextAppointment
        {
            get
            {
                var node = Root.SelectSingleNode("./header/nextAppointment");
                if (node == null) return null;
                return new NextAppointment(ReadIntegerAttribute(node, "hour"), ReadIntegerAttribute(node, "minute"));
            }
        }

        /// <summary>
        /// Consulta
        /// </summary>
        public string Query => Root.SelectSingleNode("./header/query")?.InnerText;
        
        /// <summary>
        /// Descrição da Consulta
        /// </summary>
        public string Description => Root.SelectSingleNode("./header/description")?.InnerText;

        /// <summary>
        /// Alertas que ocorreram durante o processamento
        /// </summary>
        public DocumentException[] Warnings
        {
            get
            {
                var warningNodes = Root.SelectNodes("./header/warning");
                return (from XmlNode warningNode in warningNodes select ParseExceptionNode(warningNode)).ToArray();
            }
        }

        /// <summary>
        /// Chamadas válidas (para o caso de rotear em diversas chamadas)
        /// </summary>
        public string[] ValidRequests
        {
            get
            {
                var validRequests = new List<string>();
                var nodeValidRequests = Root.SelectNodes("./header/validRequest");
                if (nodeValidRequests != null)
                    validRequests.AddRange(from XmlNode nodeValidRequest in nodeValidRequests
                        select nodeValidRequest.InnerText);
                return validRequests.ToArray();
            }
        }

        protected static int? ReadIntegerAttribute(XmlNode node, string attr, int? defaultValue = 0)
        {
            var str = node.Attributes?[attr]?.Value;
            return str == null ? defaultValue : int.Parse(str);
        }

        private void AssertException()
        {
            if (Root == null) throw new Exception("O documento não retornou.");
            var node = Root.SelectSingleNode("./header/exception");
            if (node == null) return;
            throw ParseExceptionNode(node);
        }

        private DocumentException ParseExceptionNode(XmlNode node)
        {
            var code = node.Attributes?["code"]?.Value;

            var database = node.Attributes?["databaseName"] == null
                ? null
                : new Database(
                    node.Attributes?["databaseName"]?.Value,
                    node.Attributes?["databaseDescription"]?.Value,
                    node.Attributes?["databaseUrl"]?.Value);

            var table = node.Attributes?["tableName"] == null
                ? null
                : new Table(
                    database,
                    node.Attributes?["tableName"]?.Value,
                    node.Attributes?["tableDescription"]?.Value,
                    node.Attributes?["tableUrl"]?.Value);

            var parserException = new DocumentException(node.InnerText,
                node.Attributes?["push"]?.Value == "true",
                code == null ? (int) Exception.Codes.EmptyCode : int.Parse(code),
                node.Attributes?["source"]?.Value,
                node.Attributes?["id"]?.Value,
                node.Attributes?["log"]?.Value,
                table,
                node.Attributes?["query"]?.Value);

            return parserException;
        }
    }
}