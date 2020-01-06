using Newtonsoft.Json.Linq;

namespace BipbopNet.Parser
{
    public class OabAdvogado
    {
        private readonly JToken _adv;

        public string? Nome => _adv["Nome"]?.ToString();
        public string? Inscricao => _adv["Inscricao"]?.ToString();
        public string? Uf => _adv["Nome"]?.ToString();
        public string? TipoInscricao => _adv["TipoInscricao"]?.ToString();
        public string? Endereco => _adv["Endereco"]?.ToString();
        public string? Organizacao => _adv["Organizacao"]?.ToString();
        public string? Telefones => _adv["Telefones"]?.ToString();
        public string? Situacao => _adv["Situacao"]?.ToString();
        public string? Foto => _adv["Foto"]?.ToString();
        public string? Email => _adv["Email"]?.ToString();
        public bool PermiteEnviarEmail => _adv["PermiteEnviarEmail"].ToString() == "True";
        public bool PublicaEndereco => _adv["PublicaEndereco"]?.ToString() == "True";
        public bool PublicaFoto => _adv["PublicaFoto"].ToString() == "True";

        public OabAdvogado(JToken adv)
        {
            _adv = adv;
        }
    }
}