using System;
using System.Collections.Generic;
using BipbopNet.Parser;

namespace BipbopNet.Push
{
    [Serializable]
    public class Configuration : JobIdentifier, ICloneable
    {
        /// <summary>
        ///     Momento da primeira execução
        /// </summary>
        public DateTime? At;

        /// <summary>
        ///     Callback onde será enviado o documento quando concluído
        /// </summary>
        public Uri Callback;

        /// <summary>
        ///     Tempo de vida máximo do documento
        /// </summary>
        public DateTime? Expire;

        /// <summary>
        ///     Intervalo de execuções em segundos
        /// </summary>
        public int? Interval;

        /// <summary>
        ///     Versão máxima
        /// </summary>
        public int? MaxVersion;

        /// <summary>
        ///     Parâmetros de Consulta
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters;

        /// <summary>
        ///     Prioridade, quanto menor, melhor.
        /// </summary>
        public int? Priority;

        /// <summary>
        ///     Consulta
        /// </summary>
        public string Query;

        /// <summary>
        ///     Tempo entre tentativas mal-sucedidas em segundos
        /// </summary>
        public int? RetryIn;

        /// <summary>
        ///     Tags do PUSH
        /// </summary>
        public List<string> Tags;

        /// <summary>
        ///     Entrega via WebSocket?
        /// </summary>
        public bool? WebSocketDeliver = true;

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///     Cria as configurações de um PUSH com uma query e parâmetros.
        /// </summary>
        /// <param name="query">Query</param>
        /// <param name="parameters">Parâmetros</param>
        /// <returns>Trabalho</returns>
        public static Configuration factory(string query, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            return new Configuration
            {
                Query = query,
                Parameters = parameters
            };
        }

        /// <summary>
        ///     Cria as configurações de um PUSH com uma query e parâmetros.
        /// </summary>
        /// <param name="table">Tabela</param>
        /// <param name="parameters">Parâmetros</param>
        /// <returns>Trabalho</returns>
        public static Configuration factory(Table table, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            return new Configuration
            {
                Query = table.SelectString(),
                Parameters = parameters
            };
        }
    }
}