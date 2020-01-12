using System;
using System.Globalization;
using System.Xml;

namespace BipbopNet.Parser
{
    public class CourtDate
    {
        private static readonly CultureInfo CultureInfo = new CultureInfo("pt-BR");

        /// <summary>
        ///     Formato da Data String
        /// </summary>
        public readonly string Format;

        /// <summary>
        ///     Valor da Data String
        /// </summary>
        public readonly string Value;

        public CourtDate(XmlNode node)
        {
            if (node == null) return;
            Value = node.InnerText;
            Format = node.Attributes?["format"]?.Value;
        }

        public CourtDate(string value, string format = "d/m/Y")
        {
            Value = value;
            Format = format;
        }

        /// <summary>
        ///     Data
        /// </summary>
        public DateTime DateTime => DateTime.Parse(Value, CultureInfo);


        /// <summary>
        ///     Constrói a partir de um nó XML
        /// </summary>
        /// <param name="node">Nó</param>
        /// <returns>CourtDate ou NULL</returns>
        public static CourtDate FromNode(XmlNode node)
        {
            return node == null ? null : new CourtDate(node);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}