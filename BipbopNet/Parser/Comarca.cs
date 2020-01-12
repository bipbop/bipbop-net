using System.Xml;

namespace BipbopNet.Parser
{
    public class Comarca : Vara
    {
        public Comarca(XmlNode orgao) : base(orgao)
        {
        }

        public new static Comarca Factory(XmlNode node)
        {
            return node == null ? null : new Comarca(node);
        }
    }
}