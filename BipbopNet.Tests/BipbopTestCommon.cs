using System.Xml;

namespace BipbopNet.Tests
{
    public class BipbopTestCommon
    {
        protected BipbopParser ParseDocument(string str)
        {
            var document = new XmlDocument();
            document.LoadXml(str);
            return new BipbopParser(document);
        }
    }
}