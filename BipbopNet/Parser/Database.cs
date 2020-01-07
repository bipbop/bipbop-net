namespace BipbopNet.Parser
{
    /// <summary>
    /// Portal de Informações
    /// </summary>
    public class Database : TargetDescription
    {
        public Database(string name, string description, string url, string label = null) : base(name, description,
            url, label)
        {
        }
    }
}