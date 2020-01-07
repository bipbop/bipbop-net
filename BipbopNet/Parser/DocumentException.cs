using System;

namespace BipbopNet.Parser
{
    public class DocumentException : Exception
    {
        /// <summary>
        /// Código da Exceção, abrir Exception.Codes
        /// </summary>
        public readonly int Code;
        
        
        /// <summary>
        /// Código da Exceção
        /// </summary>
        public Codes ExceptionCode 
        {
            get {
                Enum.TryParse(Code.ToString(), out Codes code);   
                return code;
            }
        }
        /// <summary>
        /// Origem da Exceção caso hajam rotas
        /// </summary>
        public readonly Table From;
        
        /// <summary>
        /// Id da Exceção para DEBUG da BIPBOP
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// Código de LOG da Exceção para DEBUG da BIPBOP
        /// </summary>
        public readonly string Log;
        
        /// <summary>
        /// Origem da exceção
        /// </summary>
        public readonly string Origin;
        
        /// <summary>
        /// A exceção é válida, exemplo: Processo não existe, segredo de justiça, coisas que podem ser exibidas
        /// ao usuário.
        /// </summary>
        public readonly bool Push;
        
        /// <summary>
        /// Query que originou a exceção
        /// </summary>
        public readonly string Query;

        public DocumentException(
            string message,
            bool push = false,
            int code = (int) Codes.EmptyCode,
            string origin = null,
            string id = null,
            string log = null,
            Table from = null,
            string query = null) : base(message)
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