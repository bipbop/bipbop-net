using System.Xml;

namespace BipbopNet.Parser
{
    public class Parte
    {
        private readonly XmlNode _parte;

        public Parte(XmlNode parte)
        {
            _parte = parte;
        }

        public string? Documento => _parte.Attributes["documento"]?.Value;
        public string? Endereco => _parte.Attributes["endereco"]?.Value;
        public string? Tipo => _parte.Attributes["tipo"]?.Value;
        public string? Nome => _parte.InnerText;
    }
}