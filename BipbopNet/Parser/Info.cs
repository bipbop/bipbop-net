using System;
using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    ///     Descrição de Consultas Disponíveis
    /// </summary>
    public class Info : BipbopDocument
    {
        public Info(XmlDocument document) : base(document)
        {
        }

        /// <summary>
        ///     Portais de Consulta
        /// </summary>
        public DatabaseDescription[] Databases =>
            (from XmlNode i in Root.SelectNodes("./body/database") select new DatabaseDescription(i)).ToArray();

        /// <summary>
        ///     Procura uma Tabela
        /// </summary>
        /// <param name="targetDatabase">Database</param>
        /// <param name="targetTable">Table</param>
        /// <returns>Tabela</returns>
        public TableDescription findTable(string targetDatabase, string targetTable)
        {
            if (string.IsNullOrEmpty(targetDatabase))
                throw new ArgumentException("Value cannot be null or empty.", nameof(targetDatabase));
            if (string.IsNullOrEmpty(targetTable))
                throw new ArgumentException("Value cannot be null or empty.", nameof(targetTable));
            var database = Databases.First(databaseDescription =>
                string.Compare(databaseDescription.Name, targetDatabase, StringComparison.OrdinalIgnoreCase) == 0);
            return database.Tables.First(tableDescription =>
                string.Compare(tableDescription.Name, targetTable, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}