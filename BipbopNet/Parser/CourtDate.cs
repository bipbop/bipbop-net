using System;
using System.Globalization;
using System.Xml;

namespace BipbopNet.Parser
{
    public class CourtDate
    {
        public readonly string? Value;
        public readonly string? Format;
        private static readonly CultureInfo CultureInfo = new CultureInfo("pt-BR");
        public DateTime DateTime => DateTime.Parse(Value, CultureInfo);
        

        public static CourtDate FromNode(XmlNode? node)
        {
            return node == null ? null : new CourtDate(node);
        }
        
        public CourtDate(XmlNode? node)
        {
            if (node == null) return;
            Value = node.InnerText;
            Format = node.Attributes["format"]?.Value;
        }
        
        public CourtDate(string? value, string? format = "d/m/Y")
        {
            Value = value;
            Format = format;
        }
        
        public override string? ToString()
        {
            return Value;
        }
    }
}