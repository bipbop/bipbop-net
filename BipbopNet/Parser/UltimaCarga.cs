using System;
using System.Xml;

namespace BipbopNet.Parser
{
    public class UltimaCarga
    {
        private readonly XmlNode _node;

        public UltimaCarga(XmlNode node)
        {
            _node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public string Remessa => _node.SelectSingleNode("./remessa")?.InnerText;
        public string Origem => _node.SelectSingleNode("./origem")?.InnerText;
        public string Destino => _node.SelectSingleNode("./destino")?.InnerText;
        public string Recebimento => _node.SelectSingleNode("./recebimento")?.InnerText;

        public UltimaCarga Factory(XmlNode node)
        {
            return node == null ? null : new UltimaCarga(node);
        }
    }
}