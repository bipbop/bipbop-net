using System;
using System.Collections.Generic;
using System.Xml;

namespace BipbopNet.Push
{
    public class PushConfiguration: PushIdentifier, ICloneable
    {
        public string? Query;
        public List<KeyValuePair<string, string>>? Parameters;
        public string? Callback;
        public int? Priority;
        public int? Interval;
        public int? RetryIn;
        public int? MaxVersion;
        public DateTime? At;
        public DateTime? Expire;
        public List<string>? Tags;
        public bool? WebSocketDeliver;


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}