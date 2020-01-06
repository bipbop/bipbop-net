using BipbopNet.Parser;

namespace BipbopNet
{
    public class QueryException : DocumentException
    {
        public QueryException(string message, bool push = false, int code = -1, string? origin = null,
            string? id = null, string? log = null, Table? from = null, string? query = null) : base(message, push,
            code, origin, id, log, from, query)
        {
        }
    }
}