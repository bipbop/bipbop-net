using System;
using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    ///     Advogado do Processo
    /// </summary>
    public class Advogado
    {
        private readonly XmlNode _advogado;


        /// <summary>
        ///     Inicializa um Advogado
        /// </summary>
        /// <param name="advogado">Nó do advogado no XML</param>
        public Advogado(XmlNode advogado)
        {
            _advogado = advogado ?? throw new ArgumentNullException(nameof(advogado));
        }

        /// <summary>
        ///     CPF do Advogado
        /// </summary>
        public string Documento => _advogado.Attributes?["documento"]?.Value;

        /// <summary>
        ///     Código OAB do Advogado
        /// </summary>
        public string Oab => _advogado.Attributes?["OAB"]?.Value;

        /// <summary>
        ///     Parte que o Advogado defende
        /// </summary>
        public string Parte => _advogado.Attributes?["parte"]?.Value;

        /// <summary>
        ///     Endereço do Advogado
        /// </summary>
        public string Endereco => _advogado.Attributes?["endereco"]?.Value;

        /// <summary>
        ///     Nome do Advogado
        /// </summary>
        public string Nome => _advogado.InnerText;

        public override string ToString()
        {
            return Nome;
        }
    }
}