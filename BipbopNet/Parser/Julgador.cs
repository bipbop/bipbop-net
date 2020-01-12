using System.Xml;

namespace BipbopNet.Parser
{
    public class Julgador
    {
        public string Nome;
        public string Tipo;

        public static Julgador Factory(XmlNode node)
        {
            if (node == null) return null;
            return new Julgador
            {
                Nome = node.InnerText,
                Tipo = node.Attributes?["tipo"]?.Value
            };
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}