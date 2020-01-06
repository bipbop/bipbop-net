using System.Linq;
using Newtonsoft.Json.Linq;

namespace BipbopNet.Parser
{
    public class OabConsulta
    {
        private readonly JObject _obj;
        
        public OabAdvogado[] Advogados => (from adv in _obj["who"] select new OabAdvogado(adv)).ToArray();  
        public OabPush[] Pushes => (from push in _obj["pushes"] select new OabPush(push)).ToArray();  

        public OabConsulta(JObject obj)
        {
            _obj = obj;
        }
    }
}