using System;
using System.Net;
using System.Xml;
using BipbopNet.Parser;
using Exception = System.Exception;

namespace BipbopNet.Push
{
    public class ListenerEvent: EventArgs
    {
        public readonly string? Company;
        public readonly int? Version;
        public readonly string? ApiKey;
        public readonly bool Exception;
        public readonly string? MemoryId; 
        public readonly PushIdentifier? Push;
        public readonly Parser.Exception? ParserException;
        public readonly BipbopDocument? Document;
        
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
            Version = request.Headers["X-BIPBOP-VERSION"] != null ? 
                int.Parse(request.Headers["X-BIPBOP-VERSION"]) :
                (int?) null;
            MemoryId = request.Headers["X-BIPBOP-MEMORY-ID"];
            var xmlDocument = new XmlDocument();
            var str = GetRequestPostData(request);
            if (str != null) xmlDocument.LoadXml(str);
            try
            {
                Document = new BipbopDocument(xmlDocument);
            }
            catch (Parser.Exception e)
            {
                ParserException = e;
            }
        }
        
        private static string? GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody) return null;
            using System.IO.Stream body = request.InputStream;
            using System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding);
            return reader.ReadToEnd();
        }
    }
}