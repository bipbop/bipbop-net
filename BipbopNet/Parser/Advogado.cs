using System.Xml;

namespace BipbopNet.Parser
{
    public class Advogado
    {
        private readonly XmlNode _advogado;

        public Advogado(XmlNode advogado)
        {
            _advogado = advogado;
        }

        public string? Documento => _advogado.Attributes["documento"]?.Value;
        public string? Oab => _advogado.Attributes["OAB"]?.Value;
        public string? Parte => _advogado.Attributes["parte"]?.Value;
        public string? Endereco => _advogado.Attributes["endereco"]?.Value;
        public string? Nome => _advogado.InnerText;
    }
}