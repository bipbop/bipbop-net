using System.Xml;

namespace BipbopNet.Parser
{
    public class Tag
    {
        private readonly XmlNode _tagNode;

        public Tag(XmlNode node)
        {
            _tagNode = node;
        }

        public string? Data => _tagNode.Attributes["data"]?.Value;
        public string? Tipo => _tagNode.Attributes["tipo"]?.Value;

        public override string ToString()
        {
            return _tagNode.InnerText;
        }
    }
}