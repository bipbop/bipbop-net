using BipbopNet.Push;
using Newtonsoft.Json.Linq;

namespace BipbopNet.Parser
{
    /// <summary>
    ///     Identificador de PUSH da OAB
    /// </summary>
    public class OabJob : JobIdentifier
    {
        public OabJob(JToken push)
        {
            Label = push["push"]?["label"].ToString();
            Id = push["push"]?["_id"]?["$id"].ToString();
        }
    }
}