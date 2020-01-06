using System;
using System.Xml;

namespace BipbopNet.Parser
{
    public class Andamento
    {
        private readonly XmlNode _andamento;
        /**
         * Descrição do andamento processual na Justiça
         */
        public string? Descricao => _andamento.SelectSingleNode("./descricao")?.InnerText;
        public string? Tipo => _andamento.SelectSingleNode("./tipo")?.InnerText;
        public string? TipoIncidente => _andamento.SelectSingleNode("./tipo_incidente")?.InnerText;
        public string? TipoAndamento => _andamento.SelectSingleNode("./tipo_andamento")?.InnerText;
        public string? UrlDocumento => _andamento.SelectSingleNode("./url_documento")?.InnerText;
        public string? NumeroAndamento => _andamento.SelectSingleNode("./numero_andamento")?.InnerText;
        public string? CodigoNacional => _andamento.SelectSingleNode("./codigoNacional")?.InnerText;
        public string? Ordenacao => _andamento.SelectSingleNode("./ordenacao")?.InnerText;
        public string? Acao => _andamento.SelectSingleNode("./acao")?.InnerText;
        
        public CourtDate? Data => CourtDate.FromNode(_andamento.SelectSingleNode("./data"));
        public CourtDate? Autuacao => CourtDate.FromNode(_andamento.SelectSingleNode("./autuacao"));
        public CourtDate? Distribuicao => CourtDate.FromNode(_andamento.SelectSingleNode("./distribuicao"));

        public override string ToString()
        {
            return $"{this.Data} - {this.Descricao}";
        }

        public Classe? Classe
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
                return Int32.Parse(instancia);
            }
        }

        public Andamento(XmlNode andamento)
        {
            _andamento = andamento;
        }
    }
}