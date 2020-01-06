namespace BipbopNet.Push
{
    public class ListParameters
    {
        public ListParameters(string filterTag = null, string lastId = null, int skip = 0, int limit = 10,
            int? version = null)
        {
            FilterTag = filterTag;
            LastId = lastId;
            Skip = skip;
            Limit = limit;
            Version = version;
        }

        public string FilterTag { get; }
        public string LastId { get; }
        public int Skip { get; }
        public int Limit { get; }
        public int? Version { get; }
    }
}