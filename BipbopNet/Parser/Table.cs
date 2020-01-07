namespace BipbopNet.Parser
{
    /// <summary>
    /// Descreve uma Tabela
    /// </summary>
    public class Table : TargetDescription
    {
        public readonly Database Database;

        public Table(Database database, string name, string description, string url, string label = null) : base(
            name, description, url, label)
        {
            Database = database;
        }

        /// <summary>
        /// Retorna uma consulta de SELECT
        /// </summary>
        /// <returns></returns>
        public string SelectString()
        {
            return $"SELECT FROM '{Database.Name}'.'{Name}'";
        }
    }
}