using System;
using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    /// Andamento do Processo
    /// </summary>
    public class Andamento
    {
        private readonly XmlNode _andamento;

        public Andamento(XmlNode andamento)
        {
            _andamento = andamento ?? throw new ArgumentNullException(nameof(andamento));
        }

        /// <summary>
        /// Descrição do andamento processual na Justiça
        /// </summary>
        public string Descricao => _andamento.SelectSingleNode("./descricao")?.InnerText;

        /// <summary>
        /// Tipo, poucos portais preenchem a informação
        /// </summary>
        public string Tipo => _andamento.SelectSingleNode("./tipo")?.InnerText;

        /// <summary>
        /// Tipo do incidente, poucos portais preenchem a informação
        /// </summary>
        public string TipoIncidente => _andamento.SelectSingleNode("./tipo_incidente")?.InnerText;

        /// <summary>
        /// Tipo do andamento, poucos portais preenchem a informação
        /// </summary>
        public string TipoAndamento => _andamento.SelectSingleNode("./tipo_andamento")?.InnerText;

        /// <summary>
        /// URL do documento anexo ao andamento
        /// </summary>
        public string UrlDocumento => _andamento.SelectSingleNode("./url_documento")?.InnerText;

        /// <summary>
        /// Número do Andamento
        /// </summary>
        public string NumeroAndamento => _andamento.SelectSingleNode("./numero_andamento")?.InnerText;

        /// <summary>
        /// Código Nacional
        /// </summary>
        public string CodigoNacional => _andamento.SelectSingleNode("./codigoNacional")?.InnerText;

        /// <summary>
        /// Ordenação
        /// </summary>
        public string Ordenacao => _andamento.SelectSingleNode("./ordenacao")?.InnerText;

        /// <summary>
        /// Ação
        /// </summary>
        public string Acao => _andamento.SelectSingleNode("./acao")?.InnerText;

        /// <summary>
        /// Data do Andamento
        /// </summary>
        public CourtDate Data => CourtDate.FromNode(_andamento.SelectSingleNode("./data"));

        /// <summary>
        /// Data da Autuação
        /// </summary>
        public CourtDate Autuacao => CourtDate.FromNode(_andamento.SelectSingleNode("./autuacao"));

        /// <summary>
        /// Data da Distribuição do Andamento
        /// </summary>
        public CourtDate Distribuicao => CourtDate.FromNode(_andamento.SelectSingleNode("./distribuicao"));

        public Classe Classe
        {
            get
            {
                var classe = _andamento.SelectSingleNode("./classe");
                return classe == null ? null : new Classe(classe);
            }
        }

        public int? Instancia
        {
            get
            {
                var instancia = _andamento.SelectSingleNode("./instancia")?.InnerText;
                if (instancia == null) return null;
                return int.Parse(instancia);
            }
        }

        public override string ToString()
        {
            return $"{Data} - {Descricao}";
        }
    }
}