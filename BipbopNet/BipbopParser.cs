using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace BipbopNet
{
    public class BipbopParser
    {
        /**
         * Nó principal
         */
        protected readonly XmlNode _root;

        protected CultureInfo CultureInfo = new CultureInfo("pt-BR");

        public BipbopParser(XmlDocument document)
        {
            _root = document.DocumentElement;
            AssertException();
        }

        public int? ResourceUse => ReadIntegerAttribute(_root, "resourceUse");
        public int? SpecialResourceUse => ReadIntegerAttribute(_root, "specialResourceUse");

        public int? ExecutionTime
        {
            get
            {
                var executionTime = _root.SelectSingleNode("./header/execution-time")?.InnerText;
                return executionTime != null ? int.Parse(executionTime) : 0;
            }
        }

        public DateTime? DateTime
        {
            get
            {
                var dateTime = _root.SelectSingleNode("./header/date-time");
                if (dateTime == null) return null;
                return System.DateTime.Parse(dateTime.InnerText);
            }
        }


        public NextAppointment? NextAppointment
        {
            get
            {
                var node = _root.SelectSingleNode("./header/nextAppointment");
                if (node == null) return null;
                return new NextAppointment(ReadIntegerAttribute(node, "hour"), ReadIntegerAttribute(node, "minute"));
            }
        }

        public string? Query => _root.SelectSingleNode("./header/query")?.InnerText;
        public string? Description => _root.SelectSingleNode("./header/description")?.InnerText;

        public BipbopParserException[] Warnings
        {
            get
            {
                List<BipbopParserException> warnings = new List<BipbopParserException>();
                var warningNodes = _root.SelectNodes("./header/warning");
                foreach (XmlNode warningNode in warningNodes) warnings.Add(ParseExceptionNode(warningNode));

                return warnings.ToArray();
            }
        }

        public string[] ValidRequests
        {
            get
            {
                List<string> validRequests = new List<string>();
                var nodeValidRequests = _root.SelectNodes("./header/validRequest");
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
            var node = _root.SelectSingleNode("./header/exception");
            if (node == null) return;
            throw ParseExceptionNode(node);
        }

        private BipbopParserException ParseExceptionNode(XmlNode node)
        {
            var code = node.Attributes["code"]?.Value;

            var database = new Database(
                node.Attributes["databaseName"]?.Value,
                node.Attributes["databaseDescription"]?.Value,
                node.Attributes["databaseUrl"]?.Value);

            var table = new Table(
                database,
                node.Attributes["tableName"]?.Value,
                node.Attributes["tableDescription"]?.Value,
                node.Attributes["tableUrl"]?.Value);

            var parserException = new BipbopParserException(node.InnerText,
                node.Attributes["push"]?.Value == "true",
                code == null ? (int) BipbopException.Codes.EmptyCode : int.Parse(code),
                node.Attributes["source"]?.Value,
                node.Attributes["id"]?.Value,
                node.Attributes["log"]?.Value,
                table,
                node.Attributes["query"]?.Value);

            return parserException;
        }
    }
}