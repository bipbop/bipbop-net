using System;
using System.Xml;

namespace BipbopNet.Parser
{
    public class Vara
    {
        private readonly XmlNode _vara;

        public Vara(XmlNode orgao)
        {
            _vara = orgao ?? throw new ArgumentNullException(nameof(orgao));
        }

        /// <summary>
        ///     Código da Vara
        /// </summary>
        public string Codigo => _vara.Attributes?["codigo"]?.Value;

        /// <summary>
        ///     Sigla da Vara
        /// </summary>
        public string Sigla => _vara.Attributes?["sigla"]?.Value;

        /// <summary>
        ///     Nome da Vara
        /// </summary>
        public string Name => _vara.InnerText;

        public static Vara Factory(XmlNode node)
        {
            return node == null ? null : new Vara(node);
        }

        public override string ToString()
        {
            return _vara.InnerText;
        }
    }
}