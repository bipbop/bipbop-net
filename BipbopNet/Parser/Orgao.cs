using System.Xml;

namespace BipbopNet.Parser
{
    public class Orgao: Vara
    {
        public Orgao(XmlNode orgao) : base(orgao)
        {
        }

        public new static Orgao Factory(XmlNode node)
        {
            return node == null ? null : new Orgao(node);
            
        }
    }
}