using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    ///     Portal de Informações
    /// </summary>
    public class DatabaseDescription : Database
    {
        private readonly XmlNode _xmlNode;

        public DatabaseDescription(XmlNode xmlNode) : base(
            xmlNode.Attributes?["name"]?.Value,
            xmlNode.Attributes?["description"]?.Value,
            xmlNode.Attributes?["url"]?.Value,
            xmlNode.Attributes?["label"]?.Value)
        {
            _xmlNode = xmlNode;
        }

        /// <summary>
        ///     Páginas de Informação
        /// </summary>
        public IEnumerable<TableDescription> Tables =>
            (from XmlNode table in _xmlNode.SelectNodes("./table") select new TableDescription(this, table)).ToArray();
    }
}