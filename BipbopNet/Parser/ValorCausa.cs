using System.Globalization;
using System.Xml;

namespace BipbopNet.Parser
{
    public class ValorCausa
    {
        public readonly string UnidadeMonetaria = "R$";
        public readonly string Value;
        public decimal AsDecimal => decimal.Parse(Value, NumberStyles.Currency);
        public ValorCausa(XmlNode node)
        {
            var unidade = node.Attributes["unidade_monetaria"]?.Value;
            if (unidade != null) UnidadeMonetaria = unidade;
            Value = node.InnerText;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}