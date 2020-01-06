using BipbopNet.Push;
using Newtonsoft.Json.Linq;

namespace BipbopNet.Parser
{
    public class OabPush : PushIdentifier
    {
        public OabPush(JToken push)
        {
            Label = push["push"]?["label"].ToString();
            Id = push["push"]?["_id"]?["$id"].ToString();
        }
    }
}