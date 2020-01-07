using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    /// Descreve uma Tabela do INFO.INFO
    /// </summary>
    public class TableDescription : Table
    {
        private readonly XmlNode _xmlNode;

        public TableDescription(DatabaseDescription db, XmlNode xmlNode) : base(db, xmlNode.Attributes?["name"]?.Value,
            xmlNode.Attributes?["description"]?.Value, xmlNode.Attributes?["url"]?.Value,
            xmlNode.Attributes?["label"]?.Value)
        {
            _xmlNode = xmlNode;
        }

        public string[] Types => _xmlNode.Attributes?["type"]?.Value.Split(',');
        public bool Upload => _xmlNode.Attributes?["mainField"]?.Value == "true";

        public IEnumerable<FieldDescription> Fields =>
            (from XmlNode i in _xmlNode.SelectNodes("./field") select new FieldDescription(this, i)).ToArray();
    }
}