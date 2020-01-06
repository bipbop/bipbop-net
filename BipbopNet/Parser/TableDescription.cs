using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    public class TableDescription : Table
    {
        private readonly XmlNode _xmlNode;

        public string[]? Types => _xmlNode.Attributes["type"]?.Value.Split(',');
        public bool Upload => _xmlNode.Attributes["mainField"]?.Value == "true";

        public FieldDescription[] Fields =>
            (from XmlNode i in _xmlNode.SelectNodes("./field") select new FieldDescription(this, i)).ToArray();

        public TableDescription(DatabaseDescription db, XmlNode xmlNode) : base(db, xmlNode.Attributes["name"]?.Value,
            xmlNode.Attributes["description"]?.Value, xmlNode.Attributes["url"]?.Value, xmlNode.Attributes["label"]?.Value)
        {
            _xmlNode = xmlNode;
        }
    }
}