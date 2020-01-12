using System.Xml;

namespace BipbopNet.Parser
{
    public class Natureza : Vara
    {
        public Natureza(XmlNode orgao) : base(orgao)
        {
        }

        public new static Natureza Factory(XmlNode node)
        {
            return node == null ? null : new Natureza(node);
        }
    }
}