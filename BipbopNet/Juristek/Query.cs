using System;
using System.Collections.Generic;
using System.Linq;
using BipbopNet.Parser;
using Exception = BipbopNet.Parser.Exception;

namespace BipbopNet.Juristek
{
    /// <summary>
    /// Factory de Query da Juristek
    /// </summary>
    public class Query
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters => _parameters.ToArray();
        public readonly IEnumerable<KeyValuePair<string, string>> _parameters;
        public readonly Table Table;
        public readonly bool Upload;

        public Query(Table table, IEnumerable<KeyValuePair<string, string>> parameters = null, bool upload = false)
        {
            Upload = upload;
            Table = table;
            _parameters = parameters ?? new List<KeyValuePair<string, string>>();
            Validate();
        }

        /// <summary>
        /// Tabela de Consulta CNJ
        /// </summary>
        private static Table CnjDescriptionTable =>
            new Table(
                new Database("CNJ", "Consulta através da numeração CNJ",
                    "https://www.cnj.jus.br/programas-e-acoes/numeracao-unica/"), "PROCESSO",
                "Consulta através da numeração CNJ", "https://www.cnj.jus.br/programas-e-acoes/numeracao-unica/");


        /// <summary>
        /// Consulta de Número CNJ na Juristek
        /// </summary>
        /// <param name="processo">Número de Processo CNJ</param>
        /// <param name="userParameters">Parâmetros de Usuário</param>
        /// <returns>Query</returns>
        /// <exception cref="QueryException"></exception>
        public static Query Cnj(Processo processo, IEnumerable<KeyValuePair<string, string>> userParameters = null)
        {
            var parameters = userParameters?.ToList() ?? new List<KeyValuePair<string, string>>();
            if (processo.Table == null)
                throw new QueryException("Não há uma tabela configurada no processo solicitado",
                    false,
                    (int) Exception.Codes.MissingArgument);
            if (string.IsNullOrEmpty(processo.NumeroProcesso))
                throw new QueryException("Não há um número de processo configurado",
                    false,
                    (int) Exception.Codes.MissingArgument);
            parameters.Add(new KeyValuePair<string, string>("numero_processo", processo.NumeroProcesso));
            return new Query(processo.Table, parameters);
        }

        /// <summary>
        /// Realiza uma Consulta CNJ
        /// </summary>
        /// <param name="numeroProcesso">Número de Processo</param>
        /// <param name="userParameters">Parâmetros de Usuário</param>
        /// <returns></returns>
        public static Query Cnj(string numeroProcesso, IEnumerable<KeyValuePair<string, string>> userParameters = null)
        {
            var parameters = userParameters?.ToList() ?? new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("NUMERO_PROCESSO", numeroProcesso));
            return new Query(CnjDescriptionTable, parameters);
        }

        private void Validate()
        {
            if (Table.GetType() != typeof(TableDescription)) return;
            var tableDescription = (TableDescription) Table;
            foreach (var field in tableDescription.Fields)
            {
                var parameter = _parameters?.FirstOrDefault(p =>
                                    string.Equals(p.Key, field.Name, StringComparison.OrdinalIgnoreCase)) ?? default;
                if (parameter.Equals(default(KeyValuePair<string, string>)))
                {
                    if (field.Required || field.MainField)
                        throw new QueryException($"O parâmetro '{field.Name}' não foi preenchido",
                            code: (int) Exception.Codes.MissingArgument, push: true, @from: Table);

                    continue;
                }

                if (field.Select && field.Options.All(p => p.Value != parameter.Value))
                    throw new QueryException(
                        $"O parâmetro '{field.Name}' não foi preenchido com uma opção válida",
                        code: (int) Exception.Codes.InvalidArgument, push: true, @from: Table);
            }
        }

        private static string Escape(string str)
        {
            return str == null ? string.Empty : str.Replace("'", "");
        }

        /// <summary>
        /// Cria um QUERY
        /// </summary>
        /// <returns>Query</returns>
        public override string ToString()
        {
            var query = Table.SelectString();
            var useParameters = _parameters.ToList()
                .Concat(new[] {new KeyValuePair<string, string>("UPLOAD", Upload ? "TRUE" : "FALSE")});
            var parameters = from parameter in useParameters
                select $"'{Escape(parameter.Key.ToUpper())}' = '{Escape(parameter.Value)}'";
            var join = string.Join(" AND ", parameters);
            return !_parameters.Any() ? query : $"{query} WHERE {join}";
        }
    }
}