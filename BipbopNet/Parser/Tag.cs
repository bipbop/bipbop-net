using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    ///     Tag do Processo
    /// </summary>
    public class Tag
    {
        private readonly XmlNode _tagNode;

        public Tag(XmlNode node)
        {
            _tagNode = node;
        }

        /// <summary>
        ///     Data da Tag
        /// </summary>
        public string Data => _tagNode.Attributes?["data"]?.Value;

        /// <summary>
        ///     Tipo da Tag
        /// </summary>
        public string Tipo => _tagNode.Attributes?["tipo"]?.Value;

        /// <summary>
        ///     Valor da TAG
        /// </summary>
        /// <returns>Valor da TAG</returns>
        public override string ToString()
        {
            return _tagNode.InnerText;
        }
    }
}