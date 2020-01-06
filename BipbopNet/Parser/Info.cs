using System.Linq;
using System.Xml;

namespace BipbopNet.Parser
{
    public class Info : BipbopDocument
    {
        public Info(XmlDocument document) : base(document)
        {
        }

        public DatabaseDescription[] Databases =>
            (from XmlNode i in Root.SelectNodes("./body/database") select new DatabaseDescription(i)).ToArray();
    }
}