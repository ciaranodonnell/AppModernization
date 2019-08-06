using Newtonsoft.Json;

namespace CDC.Messaging.Core.Serializers
{
    public class JSONSerializer : StringBasedSerializer
    {
        public override T DeserializeFromString<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public override string SerializeToString<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
