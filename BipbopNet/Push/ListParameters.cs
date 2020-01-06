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

        public string FilterTag { get; private set; }
        public string LastId { get; private set; }
        public int Skip { get; private set; }
        public int Limit { get; private set; }
        public int? Version { get; private set; }
    }
}