using System;
using System.Globalization;
using System.Xml;

namespace BipbopNet.Parser
{
    public class ValorCausa
    {
        private static readonly CultureInfo CultureInfo = new CultureInfo("pt-BR");

        /// <summary>
        ///     Unidade Monetária da Causa
        /// </summary>
        public readonly string UnidadeMonetaria = "R$";

        /// <summary>
        ///     Valor como String
        /// </summary>
        public readonly string Value;

        public ValorCausa(XmlNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            var unidade = node.Attributes?["unidade_monetaria"]?.Value;
            if (unidade != null) UnidadeMonetaria = unidade;
            Value = node.InnerText;
        }

        /// <summary>
        ///     Valor como Decimal
        /// </summary>
        public decimal AsDecimal => decimal.Parse(Value, NumberStyles.Currency, CultureInfo);

        /// <summary>
        ///     Criação do Valor da Causa
        /// </summary>
        /// <param name="node">Valor da Causa</param>
        /// <returns></returns>
        public static ValorCausa Factory(XmlNode node)
        {
            if (node == null) return null;
            return new ValorCausa(node);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}