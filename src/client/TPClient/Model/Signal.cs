using Newtonsoft.Json;

namespace TPClient.Model.Api
{
    public class Signal
    {
        [JsonProperty("hash")]
        public uint Hash { get; set; }

        [JsonProperty("sensor")]
        public bool SensorStatus { get; set; }
    }
}
