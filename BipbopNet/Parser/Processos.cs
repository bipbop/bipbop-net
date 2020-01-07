using System.Collections.Generic;
using System.Xml;

namespace BipbopNet.Parser
{
    /// <summary>
    /// Documento BIPBOP que contém Processos
    /// </summary>
    public class Processos : BipbopDocument
    {
        public Processos(XmlDocument document) : base(document)
        {
        }

        
        /// <summary>
        /// Resgata os processos
        /// </summary>
        public IEnumerable<Processo> Retrieve
        {
            get
            {
                var processos = Root.SelectNodes("./body/processo");
                if (processos != null)
                    foreach (XmlNode processo in processos)
                        yield return new Processo(Document, processo);
            }
        }
    }
}