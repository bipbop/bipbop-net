using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    public class FieldDescription
    {
        public readonly TableDescription Table;
        private readonly XmlNode _xmlNode;

        public string? Name => _xmlNode.Attributes["name"]?.Value;
        public string? Caption => _xmlNode.Attributes["caption"]?.Value;
        public string? Mask => _xmlNode.Attributes["mask"]?.Value;
        public string? Description => _xmlNode.Attributes["description"]?.Value;
        public bool Required => _xmlNode.Attributes["required"]?.Value == "false";
        public bool MainField => _xmlNode.Attributes["mainField"]?.Value == "true";
        public bool Select => _xmlNode.Attributes["name"]?.Value == "true";
        
        public string[] AlternativeMask => (from XmlNode i in _xmlNode.SelectNodes("./alternative_mask") select i.InnerText).ToArray();
        public KeyValuePair<string, string>[] Options => (from XmlNode i in _xmlNode.SelectNodes("./alternative_mask") select 
            new KeyValuePair<string?,string?>(i.InnerText, i.Attributes["value"]?.Value ?? i.InnerText)).ToArray();

        public FieldDescription(TableDescription table, XmlNode xmlNode)
        {
            Table = table;
            _xmlNode = xmlNode;
        }
    }
}