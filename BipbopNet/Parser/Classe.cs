using System.Xml;

namespace BipbopNet.Parser
{
    public class Classe
    {
        public readonly string? Sigla;
        public readonly string Value;


        public Classe(XmlNode node)
        {
            Value = node.InnerText;
            Sigla = node.Attributes["sigla"]?.Value;
        }

        public Classe(string value, string? sigla)
        {
            Value = value;
            Sigla = sigla;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}