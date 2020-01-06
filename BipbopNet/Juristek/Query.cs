using System;
using System.Collections.Generic;
using System.Linq;
using BipbopNet.Parser;
using Exception = BipbopNet.Parser.Exception;

namespace BipbopNet.Juristek
{
    public class Query
    {
        public readonly bool Upload;
        public readonly Table Table;
        public readonly IEnumerable<KeyValuePair<string, string>> Parameters;

        public Query(Table table, IEnumerable<KeyValuePair<string, string>> parameters = null, bool upload = false)
        {
            Upload = upload;
            Table = table;
            Parameters = parameters ?? new List<KeyValuePair<string, string>>();
            Validate();
        }

        private static Table QueryCnj =>
            new Table(
                new Database("CNJ", "Consulta através da numeração CNJ",
                    "https://www.cnj.jus.br/programas-e-acoes/numeracao-unica/"), "PROCESSO",
                "Consulta através da numeração CNJ", "https://www.cnj.jus.br/programas-e-acoes/numeracao-unica/");

        public static Query Cnj(Processo processo)
        {
            if (processo.Table == null)
                throw new QueryException("Não há uma tabela configurada no processo solicitado",
                    push: false,
                    code: (int)Exception.Codes.MissingArgument);
            if (string.IsNullOrEmpty(processo.NumeroProcesso))
                throw new QueryException("Não há um número de processo configurado",
                    push: false,
                    code: (int)Exception.Codes.MissingArgument);
            return new Query(processo.Table,
                new[] {new KeyValuePair<string, string>("numero_processo", processo.NumeroProcesso),});
        }

        public static Query Cnj(string numeroProcesso)
        {
            return new Query(QueryCnj,
                new[] {new KeyValuePair<string, string>("numero_processo", numeroProcesso),});
        }

        public void Validate()
        {
            if (Table.GetType() != typeof(TableDescription)) return;
            var tableDescription = (TableDescription) Table;
            foreach (var field in tableDescription.Fields)
            {
                var parameter = Parameters?.FirstOrDefault(p =>
                                    string.Equals(p.Key, field.Name, StringComparison.OrdinalIgnoreCase)) ?? default(KeyValuePair<string, string>);
                if (parameter.Equals(default(KeyValuePair<string, string>)))
                {
                    if (field.Required || field.MainField)
                    {
                        throw new QueryException($"O parâmetro '{field.Name}' não foi preenchido",
                            code: (int) Exception.Codes.MissingArgument, push: true, @from: Table);
                    }

                    continue;
                }

                if (field.Select && field.Options.All(p => p.Value != parameter.Value))
                {
                    throw new QueryException(
                        message: $"O parâmetro '{field.Name}' não foi preenchido com uma opção válida",
                        code: (int) Exception.Codes.InvalidArgument, push: true, @from: Table);
                }
            }
        }

        private static string Escape(string? str)
        {
            return str == null ? string.Empty : str.Replace("'", "");
        }

        public override string ToString()
        {
            var query = Table.SelectString();
            var useParameters = Parameters.ToList()
                .Concat(new[] {KeyValuePair.Create("UPLOAD", Upload ? "TRUE" : "FALSE"),});
            var parameters = from parameter in useParameters
                select $"'{Escape(parameter.Key.ToUpper())}' = '{Escape(parameter.Value)}'";
            var @join = string.Join(" AND ", parameters);
            return !Parameters.Any() ? query : $"{query} WHERE {@join}";
        }
    }
}