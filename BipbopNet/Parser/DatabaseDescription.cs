using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    public class DatabaseDescription: Database
    {
        private readonly XmlNode _xmlNode;

        public DatabaseDescription(XmlNode xmlNode) : base(
            xmlNode.Attributes["name"]?.Value, 
            xmlNode.Attributes["description"]?.Value, 
            xmlNode.Attributes["url"]?.Value,
            xmlNode.Attributes["label"]?.Value)
        {
            _xmlNode = xmlNode;
        }
        
        public TableDescription[] Tables => (from XmlNode table in _xmlNode.SelectNodes("./table") select new TableDescription(this, table)).ToArray();

    }
}