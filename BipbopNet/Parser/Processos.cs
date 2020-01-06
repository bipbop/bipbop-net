using System.Collections.Generic;
using System.Xml;

namespace BipbopNet.Parser
{
    public class Processos: BipbopDocument
    {
        public Processos(XmlDocument document) : base(document)
        {
        }
        
        public IEnumerable<Processo> Retrieve
        {
            get
            {
                
                var processos = Root.SelectNodes("./body/processo");
                foreach (XmlNode processo in processos)
                {
                    yield return new Processo(Document, processo);
                }
            }
        }
    }
}