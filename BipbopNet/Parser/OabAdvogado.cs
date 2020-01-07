using System;
using Newtonsoft.Json.Linq;

namespace BipbopNet.Parser
{
    public class OabAdvogado
    {
        private readonly JToken _adv;

        public OabAdvogado(JToken adv)
        {
            _adv = adv ?? throw new ArgumentNullException(nameof(adv));
        }

        /// <summary>
        /// Nome do Advogado
        /// </summary>
        public string Nome => _adv["Nome"]?.ToString();
        
        /// <summary>
        /// Inscrição na OAB
        /// </summary>
        public string Inscricao => _adv["Inscricao"]?.ToString();
        
        /// <summary>
        /// Estado do Advogado
        /// </summary>
        public string Uf => _adv["Uf"]?.ToString();
        
        /// <summary>
        /// Tipo de Inscrição do Advogado
        /// </summary>
        public string TipoInscricao => _adv["TipoInscricao"]?.ToString();
        
        /// <summary>
        /// Endereço do Advogado
        /// </summary>
        public string Endereco => _adv["Endereco"]?.ToString();
        
        /// <summary>
        /// Organização do Advogado
        /// </summary>
        public string Organizacao => _adv["Organizacao"]?.ToString();
        
        /// <summary>
        /// Telefones do Advogado
        /// </summary>
        public string Telefones => _adv["Telefones"]?.ToString();
        
        /// <summary>
        /// Situação do Advogado
        /// </summary>
        public string Situacao => _adv["Situacao"]?.ToString();
        
        /// <summary>
        /// Foto do Advogado em base64
        /// </summary>
        public string Foto => _adv["Foto"]?.ToString();
        
        /// <summary>
        /// Email do Advogado Criptografado
        /// </summary>
        public string Email => _adv["Email"]?.ToString();
        
        /// <summary>
        /// Permite enviar e-mail?
        /// </summary>
        public bool PermiteEnviarEmail => _adv["PermiteEnviarEmail"].ToString() == "True";
        
        /// <summary>
        /// Publica Endereço?
        /// </summary>
        public bool PublicaEndereco => _adv["PublicaEndereco"]?.ToString() == "True";
        
        /// <summary>
        /// Publica Foto?
        /// </summary>
        public bool PublicaFoto => _adv["PublicaFoto"].ToString() == "True";
    }
}