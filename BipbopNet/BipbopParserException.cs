namespace BipbopNet
{
    public class BipbopParserException : BipbopException
    {
        public readonly int Code;
        public readonly Table? From;
        public readonly string? Id;
        public readonly string? Log;
        public readonly string? Origin;
        public readonly bool Push;
        public readonly string? Query;

        public BipbopParserException(
            string message,
            bool push = false,
            int code = (int) Codes.EmptyCode,
            string? origin = null,
            string? id = null,
            string? log = null,
            Table? from = null,
            string? query = null) : base(message)
        {
            Code = code;
            Origin = origin;
            Id = id;
            Log = log;
            Push = push;
            From = from;
            Query = query;
        }
    }
}