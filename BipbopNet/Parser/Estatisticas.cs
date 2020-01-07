using System;
using System.Xml;

namespace BipbopNet.Parser
{
    public class Estatisticas
    {
        private readonly XmlNode _node;

        public int? Paginas => ReadInteger("./paginas");
        public int? Volumes => ReadInteger("./volumes");
        public int? Caixa => ReadInteger("./caixa");

        public Estatisticas(XmlNode node)
        {
            _node = node ?? throw new ArgumentNullException(nameof(node));
        }

        private int? ReadInteger(string path)
        {
            var content = _node.SelectSingleNode(path)?.InnerText;
            if (content == null) return null;
            return int.Parse(content);
        }

        public static Estatisticas Factory(XmlNode node)
        {
            return node == null ? null : new Estatisticas(node);
        }
    }
}