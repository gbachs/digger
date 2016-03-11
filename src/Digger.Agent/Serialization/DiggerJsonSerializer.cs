using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Digger.Agent.Serialization
{
    public class DiggerJsonSerializer : IJsonSerializer
    {
        private static readonly JsonSerializerSettings Settings;

        static DiggerJsonSerializer()
        {
            Settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
#if DEBUG
                Formatting = Formatting.Indented,
#else
                Formatting = Formatting.None,
#endif
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public string Serialize<T>(T item)
        {
            return JsonConvert.SerializeObject(item, Settings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}