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

        public BipbopDocument(XmlDocument document)
        {
            Document = document;
            Root = document.DocumentElement;
            AssertException();
        }

        public int? ResourceUse => ReadIntegerAttribute(Root, "resourceUse");
        public int? SpecialResourceUse => ReadIntegerAttribute(Root, "specialResourceUse");

        public int? ExecutionTime
        {
            get
            {
                var executionTime = Root.SelectSingleNode("./header/execution-time")?.InnerText;
                return executionTime != null ? int.Parse(executionTime) : 0;
            }
        }

        public DateTime? DateTime
        {
            get
            {
                var dateTime = Root.SelectSingleNode("./header/date-time");
                if (dateTime == null) return null;
                return System.DateTime.Parse(dateTime.InnerText, _cultureInfo);
            }
        }


        public NextAppointment? NextAppointment
        {
            get
            {
                var node = Root.SelectSingleNode("./header/nextAppointment");
                if (node == null) return null;
                return new NextAppointment(ReadIntegerAttribute(node, "hour"), ReadIntegerAttribute(node, "minute"));
            }
        }

        public string? Query => Root.SelectSingleNode("./header/query")?.InnerText;
        public string? Description => Root.SelectSingleNode("./header/description")?.InnerText;

        public DocumentException[] Warnings
        {
            get
            {
                var warningNodes = Root.SelectNodes("./header/warning");
                return (from XmlNode warningNode in warningNodes select ParseExceptionNode(warningNode)).ToArray();
            }
        }

        public string[] ValidRequests
        {
            get
            {
                var validRequests = new List<string>();
                var nodeValidRequests = Root.SelectNodes("./header/validRequest");
                foreach (XmlNode nodeValidRequest in nodeValidRequests)
                    validRequests.Add(nodeValidRequest.InnerText);

                return validRequests.ToArray();
            }
        }

        protected static int? ReadIntegerAttribute(XmlNode node, string attr, int? defaultValue = 0)
        {
            var str = node.Attributes[attr]?.Value;
            if (str == null) return defaultValue;
            return int.Parse(str);
        }

        private void AssertException()
        {
            if (Root == null) throw new System.Exception("Aparentemente o documento retornou nulo.");
            var node = Root.SelectSingleNode("./header/exception");
            if (node == null) return;
            throw ParseExceptionNode(node);
        }

        private DocumentException ParseExceptionNode(XmlNode node)
        {
            var code = node.Attributes["code"]?.Value;

            var database = node.Attributes["databaseName"] == null
                ? null
                : new Database(
                    node.Attributes["databaseName"]?.Value,
                    node.Attributes["databaseDescription"]?.Value,
                    node.Attributes["databaseUrl"]?.Value);

            var table = node.Attributes["tableName"] == null
                ? null
                : new Table(
                    database,
                    node.Attributes["tableName"]?.Value,
                    node.Attributes["tableDescription"]?.Value,
                    node.Attributes["tableUrl"]?.Value);

            var parserException = new DocumentException(node.InnerText,
                node.Attributes["push"]?.Value == "true",
                code == null ? (int) Exception.Codes.EmptyCode : int.Parse(code),
                node.Attributes["source"]?.Value,
                node.Attributes["id"]?.Value,
                node.Attributes["log"]?.Value,
                table,
                node.Attributes["query"]?.Value);

            return parserException;
        }
    }
}