using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    /// Classe do Processo
    /// </summary>
    public class Classe
    {
        /// <summary>
        /// Sigla CNJ
        /// </summary>
        public readonly string Sigla;
        
        /// <summary>
        /// Classe
        /// </summary>
        public readonly string Value;


        public Classe(XmlNode node)
        {
            Value = node?.InnerText;
            Sigla = node?.Attributes?["sigla"]?.Value;
        }

        public Classe(string value, string sigla)
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