using System;
using System.IO;
using System.Net;
using System.Xml;
using BipbopNet.Parser;
using Exception = BipbopNet.Parser.Exception;

namespace BipbopNet.Push
{
    public class ListenerEvent : EventArgs
    {
        public readonly string? ApiKey;
        public readonly string? Company;
        public readonly BipbopDocument? Document;
        public readonly bool Exception;
        public readonly string? MemoryId;
        public readonly Exception? ParserException;
        public readonly PushIdentifier? Push;
        public readonly int? Version;

        public ListenerEvent(HttpListenerRequest request)
        {
            Push = new PushIdentifier
            {
                Id = request.Headers["X-BIPBOP-DOCUMENT-ID"],
                Label = request.Headers["X-BIPBOP-DOCUMENT-LABEL"]
            };
            Exception = request.Headers["X-BIPBOP-EXCEPTION"] == "true";
            Company = request.Headers["X-BIPBOP-COMPANY"];
            ApiKey = request.Headers["X-BIPBOP-APIKEY"];
            Version = request.Headers["X-BIPBOP-VERSION"] != null
                ? int.Parse(request.Headers["X-BIPBOP-VERSION"])
                : (int?) null;
            MemoryId = request.Headers["X-BIPBOP-MEMORY-ID"];
            var xmlDocument = new XmlDocument();
            var str = GetRequestPostData(request);
            if (str != null) xmlDocument.LoadXml(str);
            try
            {
                Document = new BipbopDocument(xmlDocument);
            }
            catch (Exception e)
            {
                ParserException = e;
            }
        }

        private static string? GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody) return null;
            using var body = request.InputStream;
            using var reader = new StreamReader(body, request.ContentEncoding);
            return reader.ReadToEnd();
        }
    }
}