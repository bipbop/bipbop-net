using System;
using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    ///     Resposta da BIPBOP
    /// </summary>
    public class BipbopResponse
    {
        /// <summary>
        ///     Carrega quando ocorreu um erro
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        ///     Carrega quando carregamento foi com sucesso
        /// </summary>
        public readonly BipbopDocument Response;

        /// <summary>
        ///     Resposta da BIPBOP
        /// </summary>
        /// <param name="response">Resposta de Sucesso</param>
        /// <param name="exception">Resposta de Erro</param>
        private BipbopResponse(BipbopDocument response = null, Exception exception = null)
        {
            if (response == null && exception == null) throw new NullReferenceException();
            Response = response;
            Exception = exception;
        }

        public static BipbopResponse loadDocument(XmlDocument document)
        {
            try
            {
                return new BipbopResponse(new BipbopDocument(document));
            }
            catch (Exception e)
            {
                return new BipbopResponse(null, e);
            }
        }
    }
}