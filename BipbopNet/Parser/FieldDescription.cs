using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    /// Parâmetros de uma Tabela
    /// </summary>
    public class FieldDescription
    {
        private readonly XmlNode _xmlNode;
        public readonly TableDescription Table;

        public FieldDescription(TableDescription table, XmlNode xmlNode)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            _xmlNode = xmlNode ?? throw new ArgumentNullException(nameof(xmlNode));
        }

        
        /// <summary>
        /// Nome do parâmetro, o qual será usado para enviar
        /// </summary>
        public string Name => _xmlNode.Attributes?["name"]?.Value;
        /// <summary>
        /// Nome do parâmetro descritivo ao usuário
        /// </summary>
        public string Caption => _xmlNode.Attributes?["caption"]?.Value;
        /// <summary>
        /// Máscara do Botão
        /// </summary>
        public string Mask => _xmlNode.Attributes?["mask"]?.Value;
        /// <summary>
        /// Descrição
        /// </summary>
        public string Description => _xmlNode.Attributes?["description"]?.Value;
        /// <summary>
        /// É requerido?
        /// </summary>
        public bool Required => _xmlNode.Attributes?["required"]?.Value == "false";
        /// <summary>
        /// É campo principal para consulta?
        /// </summary>
        public bool MainField => _xmlNode.Attributes?["mainField"]?.Value == "true";

        /// <summary>
        /// É um COMBOBOX
        /// </summary>
        public bool Select => _xmlNode.Attributes?["name"]?.Value == "true";

        /// <summary>
        /// Máscaras Alternativas
        /// </summary>
        public string[] AlternativeMask =>
            (from XmlNode i in _xmlNode.SelectNodes("./alternative_mask") select i.InnerText).ToArray();

        /// <summary>
        /// Opções
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Options =>
            (from XmlNode i in _xmlNode.SelectNodes("./alternative_mask")
                select
                    new KeyValuePair<string, string>(i.InnerText, i.Attributes?["value"]?.Value ?? i.InnerText))
            .ToArray();
    }
}