using Newtonsoft.Json;

namespace TPClient.Model
{
    public class ChuckNorris
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
