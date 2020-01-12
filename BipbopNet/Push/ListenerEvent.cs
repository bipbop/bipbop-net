using System;
using System.IO;
using System.Net;
using System.Xml;
using BipbopNet.Parser;
using Exception = BipbopNet.Parser.Exception;

namespace BipbopNet.Push
{
    [Serializable]
    public class ListenerEvent : EventArgs
    {
        /// <summary>
        ///     Chave de API do Dono do Documento
        /// </summary>
        public readonly string ApiKey;

        /// <summary>
        ///     Dono do Documento
        /// </summary>
        public readonly string Company;

        /// <summary>
        ///     Documento
        /// </summary>
        public readonly BipbopDocument Document;

        /// <summary>
        ///     Se há exceção no documento
        /// </summary>
        public readonly bool Exception;

        /// <summary>
        ///     Identificador do PUSH
        /// </summary>
        public readonly JobIdentifier Job;

        /// <summary>
        ///     Memória do Trabalho
        /// </summary>
        public readonly string MemoryId;

        /// <summary>
        ///     Exceção do Documento
        /// </summary>
        public readonly Exception ParserException;

        /// <summary>
        ///     Versão do Documento
        /// </summary>
        public readonly int? Version;

        /// <summary>
        ///     Recebe um Evento
        /// </summary>
        /// <param name="request">Requisição</param>
        public ListenerEvent(HttpListenerRequest request)
        {
            Job = new JobIdentifier
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

        private static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody) return null;
            var body = request.InputStream;
            var reader = new StreamReader(body, request.ContentEncoding);
            return reader.ReadToEnd();
        }
    }
}