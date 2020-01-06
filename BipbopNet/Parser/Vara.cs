using System.Xml;

namespace BipbopNet.Parser
{
    public class Vara
    {
        private readonly XmlNode _vara;
        public Vara(XmlNode vara)
        {
            _vara = vara;
        }

        public string? Codigo => _vara.Attributes["codigo"]?.Value;
        public string? Sigla => _vara.Attributes["sigla"]?.Value;
        public string Value => _vara.InnerText;

        public override string ToString()
        {
            return _vara.InnerText;
        }
        
    }
}