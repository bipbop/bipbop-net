using System;
using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{


    public class Processo: BipbopDocument
    {
        private readonly XmlNode _processoNode;
        public Processo(XmlDocument document, XmlNode processoNode) : base(document)
        {
            _processoNode = processoNode;
        }

        public string? Filter => _processoNode.Attributes["filter"]?.Value;
        public string? Id => _processoNode.Attributes["id"]?.Value;
        public string? TableInstancia => _processoNode.Attributes["instancia"]?.Value;
        public string? Origin => _processoNode.Attributes["origin"]?.Value;
        public string? PushGuid => _processoNode.Attributes["pushguid"]?.Value;
        
        public string[] Assunto => (from XmlNode node in _processoNode.SelectNodes("assunto") select node.InnerText).ToArray();
        public string[] OutrosNumeros => (from XmlNode node in _processoNode.SelectNodes("outros_numeros") select node.InnerText).ToArray();
        public string[] NumeroAntigo => (from XmlNode node in _processoNode.SelectNodes("numero_antigo") select node.InnerText).ToArray();

        public Andamento[] Andamentos => (from XmlNode node in _processoNode.SelectNodes("./andamentos/andamento") select new Andamento(node)).ToArray();
        public Andamento[] Documentos => (from XmlNode node in _processoNode.SelectNodes("./documentos/documento") select new Andamento(node)).ToArray();
        public Parte[] Partes => (from XmlNode node in _processoNode.SelectNodes("./partes/parte") select new Parte(node)).ToArray();
        public Advogado[] Advogados => (from XmlNode node in _processoNode.SelectNodes("./advogados/advogado") select new Advogado(node)).ToArray();
        public Tag[] Tags => (from XmlNode node in _processoNode.SelectNodes("./tags/tag") select new Tag(node)).ToArray();

        public string? Acao => _processoNode.SelectSingleNode("./acao")?.InnerText;
        public string? Area => _processoNode.SelectSingleNode("./area")?.InnerText;
        public string? NumeroProcesso => _processoNode.SelectSingleNode("./numero_processo")?.InnerText;
        public string? Descricao => _processoNode.SelectSingleNode("./descricao")?.InnerText;
        public string? Cartorio => _processoNode.SelectSingleNode("./cartorio")?.InnerText;
        public string? Comarca => _processoNode.SelectSingleNode("./comarca")?.InnerText;
        public string? CodigoInterno => _processoNode.SelectSingleNode("./codigo_interno")?.InnerText;
        public string? Custas => _processoNode.SelectSingleNode("./custas")?.InnerText;
        public string? Fase => _processoNode.SelectSingleNode("./fase")?.InnerText;
        public string? Incidente => _processoNode.SelectSingleNode("./incidente")?.InnerText;
        public string? Julgador => _processoNode.SelectSingleNode("./julgador")?.InnerText;
        public string? Localizacao => _processoNode.SelectSingleNode("./localizacao")?.InnerText;
        public string? Observacao => _processoNode.SelectSingleNode("./observacao")?.InnerText;
        public string? OrigemProcesso => _processoNode.SelectSingleNode("./origem_processo")?.InnerText;
        public string? OrigemTribunal => _processoNode.SelectSingleNode("./origem_tribunal")?.InnerText;
        public string? Prioridade => _processoNode.SelectSingleNode("./prioridade")?.InnerText;
        public string? Situacao => _processoNode.SelectSingleNode("./situacao")?.InnerText;
        public string? Rito => _processoNode.SelectSingleNode("./rito")?.InnerText;
        public string? Solucao => _processoNode.SelectSingleNode("./solucao")?.InnerText;
        public string? Status => _processoNode.SelectSingleNode("./status")?.InnerText;
        public string? UrlProcesso => _processoNode.SelectSingleNode("./url_processo")?.InnerText;
        
        public CourtDate Autuacao => CourtDate.FromNode(_processoNode.SelectSingleNode("autuacao"));
        public CourtDate Distribuicao => CourtDate.FromNode(_processoNode.SelectSingleNode("autuacao"));
        public CourtDate Juizo => CourtDate.FromNode(_processoNode.SelectSingleNode("juizo"));
        public CourtDate Inscricao => CourtDate.FromNode(_processoNode.SelectSingleNode("inscricao"));
        
        public Table Table
        {
            get
            {
                var database = new Database(
                    _processoNode.Attributes["databaseName"]?.Value,
                    _processoNode.Attributes["databaseDescription"]?.Value,
                    _processoNode.Attributes["databaseUrl"]?.Value);

                return new Table(database, 
                    _processoNode.Attributes["tableName"]?.Value,
                    _processoNode.Attributes["tableDescription"]?.Value,
                    _processoNode.Attributes["tableUrl"]?.Value);
            }
        }
        
        public int? Instancia
        {
            get
            {
                var instancia = _processoNode.SelectSingleNode("intancia")?.InnerText;
                if (instancia == null) return null;
                return Int32.Parse(instancia);
            }
        }

        public bool? Eletronico
        {
            get
            {
                var instancia = _processoNode.SelectSingleNode("./eletronico")?.InnerText;
                if (instancia == null) return null;
                return instancia == "true";
            }
        }


        public Classe? Classe
        {
            get
            {
                var classe = _processoNode.SelectSingleNode("./classe");
                return classe == null ? null : new Classe(classe);
            }   
        }

        public ValorCausa? ValorCausa
        {
            get
            {
                var valorCausa =_processoNode.SelectSingleNode("./valor_causa");
                return valorCausa == null ? null : new ValorCausa(valorCausa);
            }
        }

        public Vara? Vara
        {
            get
            {
                var vara = _processoNode.SelectSingleNode("./vara");
                return vara == null ? null : new Vara(vara);
            }   
        }

        public override string ToString()
        {
            return NumeroProcesso;
        }
    }
}