using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace BipbopNet.Parser
{
    public class OabConsulta
    {
        private readonly JObject _obj;

        public OabConsulta(JObject obj)
        {
            _obj = obj;
        }

        /// <summary>
        /// Lista de Advogados de uma Consulta OAB
        /// </summary>
        public IEnumerable<OabAdvogado> Advogados => (from adv in _obj["who"] select new OabAdvogado(adv)).ToArray();

        /// <summary>
        /// Lista de Requisições na BIPBOP
        /// </summary>
        public IEnumerable<OabPush> Pushes => (from push in _obj["pushes"] select new OabPush(push)).ToArray();
    }
}