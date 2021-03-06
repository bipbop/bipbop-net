using System;
using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    ///     Nó da Parte em um Processo
    /// </summary>
    public class Parte
    {
        private readonly XmlNode _parte;

        public Parte(XmlNode parte)
        {
            _parte = parte ?? throw new ArgumentNullException(nameof(parte));
        }

        /// <summary>
        ///     Situação da Parte
        /// </summary>
        public string Status => _parte.Attributes?["status"]?.Value;

        /// <summary>
        ///     Numero da Parte
        /// </summary>
        public string Numero => _parte.Attributes?["numero"]?.Value;

        /// <summary>
        ///     CPF, RG ou CNPJ da parte
        /// </summary>
        public string Documento => _parte.Attributes?["documento"]?.Value;

        /// <summary>
        ///     Endereço da parte
        /// </summary>
        public string Endereco => _parte.Attributes?["endereco"]?.Value;

        /// <summary>
        ///     Tipo da parte no processo
        ///     <example>Réu</example>
        ///     <example>Embargado</example>
        ///     <example>Ativo</example>
        /// </summary>
        public string Tipo => _parte.Attributes?["tipo"]?.Value;

        /// <summary>
        ///     Nome
        /// </summary>
        public string Nome => _parte.InnerText;

        public override string ToString()
        {
            return Nome;
        }
    }
}