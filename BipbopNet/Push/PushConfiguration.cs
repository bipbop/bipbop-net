using System;
using System.Collections.Generic;
using BipbopNet.Parser;

namespace BipbopNet.Push
{
    public class PushConfiguration : PushIdentifier, ICloneable
    {
        /// <summary>
        /// Momento da primeira execução
        /// </summary>
        public DateTime? At;
        
        /// <summary>
        /// Callback onde será enviado o documento quando concluído
        /// </summary>
        public string Callback;
        
        /// <summary>
        /// Tempo de vida máximo do documento
        /// </summary>
        public DateTime? Expire;
        
        /// <summary>
        /// Intervalo de execuções em segundos
        /// </summary>
        public int? Interval;
        
        /// <summary>
        /// Versão máxima
        /// </summary>
        public int? MaxVersion;
        
        /// <summary>
        /// Parâmetros de Consulta
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters;
        
        /// <summary>
        /// Prioridade, quanto menor, melhor.
        /// </summary>
        public int? Priority;
        
        /// <summary>
        /// Consulta
        /// </summary>
        public string Query;
        
        /// <summary>
        /// Tempo entre tentativas mal-sucedidas em segundos
        /// </summary>
        public int? RetryIn;
        
        /// <summary>
        /// Tags do PUSH
        /// </summary>
        public List<string> Tags;
        
        /// <summary>
        /// Entrega via WebSocket?
        /// </summary>
        public bool? WebSocketDeliver = true;

        /// <summary>
        /// Cria as configurações de um PUSH com uma query e parâmetros.
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="parameters">Parâmetros</param>
        /// <returns>Trabalho</returns>
        public static PushConfiguration factory(string query, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            return new PushConfiguration
            {
                Query = query,
                Parameters = parameters,
            };
        }
        
        /// <summary>
        /// Cria as configurações de um PUSH com uma query e parâmetros.
        /// </summary>
        /// <param name="table">Tabela</param>
        /// <param name="parameters">Parâmetros</param>
        /// <returns>Trabalho</returns>
        public static PushConfiguration factory(Table table, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            return new PushConfiguration
            {
                Query = table.SelectString(),
                Parameters = parameters,
            };
        }
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}