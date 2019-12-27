namespace BipbopNet
{
    public class Table : TargetDescription
    {
        public readonly Database Database;

        public Table(Database database, string? name, string? description, string? url) : base(name, description, url)
        {
            Database = database;
        }
    }
}