using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    public class Processo : BipbopDocument
    {
        private readonly XmlNode _processoNode;

        public Processo(XmlDocument document, XmlNode processoNode) : base(document)
        {
            _processoNode = processoNode;
        }

        /// <summary>
        /// Filtro do Processo para diferenciar diversa tratativa
        /// </summary>
        public string Filter => _processoNode.Attributes?["filter"]?.Value;
        
        /// <summary>
        /// ID único
        /// </summary>
        public string Id => _processoNode.Attributes?["id"]?.Value;
        
        /// <summary>
        /// Instância do Portal
        /// </summary>
        public string TableInstancia => _processoNode.Attributes?["instancia"]?.Value;
        
        /// <summary>
        /// Tipo do Portal de Origem
        /// </summary>
        public string Origin => _processoNode.Attributes?["origin"]?.Value;
        
        /// <summary>
        /// ID único do PUSh
        /// </summary>
        public string PushGuid => _processoNode.Attributes?["pushguid"]?.Value;

        /// <summary>
        /// Matérias do Processo
        /// </summary>
        public string[] Assunto =>
            (from XmlNode node in _processoNode.SelectNodes("./assunto") select node.InnerText).ToArray();

        /// <summary>
        /// Outros números para o processo
        /// </summary>
        public string[] OutrosNumeros =>
            (from XmlNode node in _processoNode.SelectNodes("./outros_numeros") select node.InnerText).ToArray();

        /// <summary>
        /// Número antigo do processo
        /// </summary>
        public string[] NumeroAntigo =>
            (from XmlNode node in _processoNode.SelectNodes("./numero_antigo") select node.InnerText).ToArray();

        /// <summary>
        /// Andamentos
        /// </summary>
        public Andamento[] Andamentos => (from XmlNode node in _processoNode.SelectNodes("./andamentos/andamento")
            select new Andamento(node)).ToArray();

        /// <summary>
        /// Documentos (presentes também em andamentos)
        /// </summary>
        public Andamento[] Documentos => (from XmlNode node in _processoNode.SelectNodes("./documentos/documento")
            select new Andamento(node)).ToArray();

        /// <summary>
        /// Participantes do Processo
        /// </summary>
        public Parte[] Partes =>
            (from XmlNode node in _processoNode.SelectNodes("./partes/parte") select new Parte(node)).ToArray();

        /// <summary>
        /// Advogados do Processo
        /// </summary>
        public Advogado[] Advogados =>
            (from XmlNode node in _processoNode.SelectNodes("./advogados/advogado") select new Advogado(node))
            .ToArray();

        /// <summary>
        /// Tags do Processo
        /// </summary>
        public Tag[] Tags =>
            (from XmlNode node in _processoNode.SelectNodes("./tags/tag") select new Tag(node)).ToArray();

        /// <summary>
        /// Tipo de Ação do Processo
        /// </summary>
        public string Acao => _processoNode.SelectSingleNode("./acao")?.InnerText;

        /// <summary>
        /// Âmbito/ramo do direito
        /// </summary>
        public string Area => _processoNode.SelectSingleNode("./area")?.InnerText;

        /// <summary>
        /// Identificador do Processo
        /// </summary>
        public string NumeroProcesso => _processoNode.SelectSingleNode("./numero_processo")?.InnerText;

        /// <summary>
        /// Identificador Protocolar do Processo
        /// </summary>
        public string NumeroProtocolo => _processoNode.SelectSingleNode("./numero_protocolo")?.InnerText;
        
        /// <summary>
        /// Identificador Protocolar do Processo (NPU)
        /// Por convenção, será setado em número unico do processo
        /// qualquer número do processo que não esteja no formato número único do processo (NPU) definido pelo CNJ em 2015
        /// </summary>
        public string NumeroUnico => _processoNode.SelectSingleNode("./numero_unico")?.InnerText;
        
        /// <summary>
        /// Número complemento
        /// </summary>
        public string NumeroComplemento => _processoNode.SelectSingleNode("./numero_complemento")?.InnerText;
        
        /// <summary>
        /// Número do Recurso
        /// </summary>
        public string NumeroRecurso => _processoNode.SelectSingleNode("./numero_recurso")?.InnerText;        
        
        /// <summary>
        /// Descrição do Processo
        /// </summary>
        public string NumeroDivida => _processoNode.SelectSingleNode("./numero_divida")?.InnerText;
        
        /// <summary>
        /// Descrição do Processo
        /// </summary>
        public string Descricao => _processoNode.SelectSingleNode("./descricao")?.InnerText;

        /// <summary>
        /// Foro do Processo
        /// </summary>
        public string Foro => _processoNode.SelectSingleNode("./foro")?.InnerText;

        /// <summary>
        /// Cartório do Processo
        /// </summary>
        public string Cartorio => _processoNode.SelectSingleNode("./cartorio")?.InnerText;
        /// <summary>
        /// Corresponde ao território em que o juiz de primeiro grau irá exercer sua jurisdição e pode abranger um ou mais municípios
        /// </summary>
        public Comarca Comarca => Comarca.Factory(_processoNode.SelectSingleNode("./comarca"));
        
        /// <summary>
        /// Corresponde ao território em que o juiz de primeiro grau irá exercer sua jurisdição e pode abranger um ou mais municípios
        /// </summary>
        public Comarca ComarcaInicial => Comarca.Factory(_processoNode.SelectSingleNode("./comarca_inicial"));

        /// <summary>
        /// Código Interno do Sistema do Tribunal
        /// </summary>
        public string CodigoInterno => _processoNode.SelectSingleNode("./codigo_interno")?.InnerText;
        
        /// <summary>
        /// Custas Processuais
        /// </summary>
        public ValorCausa Custas => ValorCausa.Factory(_processoNode.SelectSingleNode("./custas"));
        
        /// <summary>
        /// Fase: momento processual. Existem duas essenciais de conhecimento (fase decisória) e de execução (cumprimento de sentença).
        /// </summary>
        public string Fase => _processoNode.SelectSingleNode("./fase")?.InnerText;

        /// <summary>
        /// Incidente processual é uma questão controversa secundária e acessória que surge no curso de um processo e que precisa ser julgada antes da decisão do mérito da causa principal.
        /// </summary>
        public Natureza Natureza => Natureza.Factory(_processoNode.SelectSingleNode("./natureza"));


        public Estatisticas Estatisticas => Estatisticas.Factory(_processoNode.SelectSingleNode("./natureza"));
        
        /// <summary>
        /// Incidente processual é uma questão controversa secundária e acessória que surge no curso de um processo e que precisa ser julgada antes da decisão do mérito da causa principal.
        /// </summary>
        public Orgao Orgao => Orgao.Factory(_processoNode.SelectSingleNode("./orgao"));
        
        /// <summary>
        /// Incidente processual é uma questão controversa secundária e acessória que surge no curso de um processo e que precisa ser julgada antes da decisão do mérito da causa principal.
        /// </summary>
        public string Incidente => _processoNode.SelectSingleNode("./incidente")?.InnerText;

        /// <summary>
        /// Julgador, juiz ou desembargador
        /// </summary>
        public Julgador Julgador => Julgador.Factory(_processoNode.SelectSingleNode("./julgador"));
        
        public bool SegredoJustica => _processoNode.SelectSingleNode("./segredo_justica") != null;
        
        /// <summary>
        /// Localização Física do Processo
        /// </summary>
        public string Localizacao => _processoNode.SelectSingleNode("./localizacao")?.InnerText;
        
        /// <summary>
        /// Observação do Processo
        /// </summary>
        public string Observacao => _processoNode.SelectSingleNode("./observacao")?.InnerText;
        
        /// <summary>
        /// Apenso do Processo
        /// </summary>
        public string Apenso => _processoNode.SelectSingleNode("./apenso")?.InnerText;
        
        /// <summary>
        /// Origem
        /// </summary>
        public string OrigemProcesso => _processoNode.SelectSingleNode("./origem_processo")?.InnerText;
        
        /// <summary>
        /// Origem do Processo
        /// </summary>
        public string OrigemTribunal => _processoNode.SelectSingleNode("./origem_tribunal")?.InnerText;
        /// <summary>
        /// Tipo de Prioridade
        /// </summary>
        public string Prioridade => _processoNode.SelectSingleNode("./prioridade")?.InnerText;
        
        public string Juizado => _processoNode.SelectSingleNode("./juizado")?.InnerText;

        /// <summary>
        /// Situação do Processo
        /// </summary>
        public string Situacao => _processoNode.SelectSingleNode("./situacao")?.InnerText;

        /// <summary>
        /// Situação do Processo
        /// </summary>
        public string LocalizacaoImovel => _processoNode.SelectSingleNode("./localizacao_imovel")?.InnerText;

        /// <summary>
        /// Rito
        /// </summary>
        public string Rito => _processoNode.SelectSingleNode("./rito")?.InnerText;
        
        /// <summary>
        /// Seção
        /// </summary>
        public string Secao => _processoNode.SelectSingleNode("./secao")?.InnerText;
        
        /// <summary>
        /// Adicional
        /// </summary>
        public string Adicional => _processoNode.SelectSingleNode("./adicional")?.InnerText;

        /// <summary>
        /// Solução
        /// </summary>
        public string Solucao => _processoNode.SelectSingleNode("./solucao")?.InnerText;
        
        /// <summary>
        /// URL do Processo
        /// </summary>
        public string UrlProcesso => _processoNode.SelectSingleNode("./url_processo")?.InnerText;

        public CourtDate Autuacao => CourtDate.FromNode(_processoNode.SelectSingleNode("./autuacao"));
        public CourtDate Distribuicao => CourtDate.FromNode(_processoNode.SelectSingleNode("./distribuicao"));
        
        public CourtDate Juizo => CourtDate.FromNode(_processoNode.SelectSingleNode("./juizo"));
        public CourtDate AndamentoInicial => CourtDate.FromNode(_processoNode.SelectSingleNode("./andamento_inicial"));

        public CourtDate DataValorCausa => CourtDate.FromNode(_processoNode.SelectSingleNode("./data_valor_causa"));

        public CourtDate Arquivamento => CourtDate.FromNode(_processoNode.SelectSingleNode("./arquivamento"));

        public CourtDate TransitoJulgado => CourtDate.FromNode(_processoNode.SelectSingleNode("./transito_julgado"));

        public CourtDate Inscricao => CourtDate.FromNode(_processoNode.SelectSingleNode("./inscricao"));

        public CourtDate Ajuizamento => CourtDate.FromNode(_processoNode.SelectSingleNode("./ajuizamento"));

        public CourtDate Audiencia => CourtDate.FromNode(_processoNode.SelectSingleNode("./audiencia"));

        public Table Table
        {
            get
            {
                var database = new Database(
                    _processoNode.Attributes?["databaseName"]?.Value,
                    _processoNode.Attributes?["databaseDescription"]?.Value,
                    _processoNode.Attributes?["databaseUrl"]?.Value);

                return new Table(database,
                    _processoNode.Attributes?["tableName"]?.Value,
                    _processoNode.Attributes?["tableDescription"]?.Value,
                    _processoNode.Attributes?["tableUrl"]?.Value);
            }
        }

        public int? Instancia
        {
            get
            {
                var instancia = _processoNode.SelectSingleNode("./intancia")?.InnerText;
                if (instancia == null) return null;
                return int.Parse(instancia);
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


        public Classe Classe
        {
            get
            {
                var classe = _processoNode.SelectSingleNode("./classe");
                return classe == null ? null : new Classe(classe);
            }
        }

        public ValorCausa ValorCausa => ValorCausa.Factory( _processoNode.SelectSingleNode("./valor_causa"));

        public Vara Vara
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