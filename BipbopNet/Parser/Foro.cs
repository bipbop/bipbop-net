using System.Xml;

namespace BipbopNet.Parser
{
    public class Foro: Vara
    {
        public Foro(XmlNode orgao) : base(orgao)
        {
        }

        public new static Comarca Factory(XmlNode node)
        {
            return node == null ? null : new Comarca(node);
            
        }
    }
}