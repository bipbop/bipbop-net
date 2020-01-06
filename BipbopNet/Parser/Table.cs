namespace BipbopNet.Parser
{
    public class Table : TargetDescription
    {
        public readonly Database Database;

        public Table(Database database, string? name, string? description, string? url, string? label = null) : base(
            name, description, url, label)
        {
            Database = database;
        }

        public string SelectString()
        {
            return $"SELECT FROM '{Database.Name}'.'{Name}'";
        }
    }
}