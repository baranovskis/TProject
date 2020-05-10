using Newtonsoft.Json;

namespace TPClient.Model
{
    public class CatFact
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
