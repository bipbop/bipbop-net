using System;
using System.Collections.Generic;

namespace BipbopNet.Push
{
    public class PushConfiguration : PushIdentifier, ICloneable
    {
        public DateTime? At;
        public string? Callback;
        public DateTime? Expire;
        public int? Interval;
        public int? MaxVersion;
        public List<KeyValuePair<string, string>>? Parameters;
        public int? Priority;
        public string? Query;
        public int? RetryIn;
        public List<string>? Tags;
        public bool? WebSocketDeliver;


        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}