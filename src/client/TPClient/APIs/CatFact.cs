using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using TPClient.Services;
using TPClient.Utilities;

namespace TPClient.APIs
{
    public class CatFact : IJokeAPI
    {
        public string Name => "Cat Facts";

        public List<string> Categories => null;

        /// <summary>
        /// Get random fact.
        /// </summary>
        /// <returns></returns>
        public string GetJoke()
        {
            var url = "https://cat-fact.herokuapp.com/facts/random";

            if (!string.IsNullOrEmpty(JokesManager.Instance.Category))
            {
                url += $"?category={JokesManager.Instance.Category}";
            }

            var client = new RestClient();
            var request = new RestRequest(url);
            var response = client.Execute(request);

            var fact = JsonConvert.DeserializeObject<Model.CatFact>(response.Content);

            if (fact == null)
            {
                Log.Instance.Error("CatFact deserialize object failed! JSON: {0}", fact);
                return string.Empty;
            }

            return fact.Text;
        }
    }
}
