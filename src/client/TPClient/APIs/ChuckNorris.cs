using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using TPClient.Services;
using TPClient.Utilities;

namespace TPClient.APIs
{
    public class ChuckNorris : IJokeAPI
    {
        public string Name => "Chuck Norris Facts";

        public List<string> Categories => new List<string>
        {
            string.Empty,
            "Animal",
            "Career",
            "Celebrity",
            "Dev",
            "Explicit",
            "Fashion",
            "Food",
            "History",
            "Money",
            "Movie",
            "Music",
            "Political",
            "Religion",
            "Science",
            "Sport",
            "Travel"
        };

        /// <summary>
        /// Get random fact.
        /// </summary>
        /// <returns></returns>
        public string GetJoke()
        {
            var url = "https://api.chucknorris.io/jokes/random";

            if (!string.IsNullOrEmpty(JokesManager.Instance.Category))
            {
                url += $"?category={JokesManager.Instance.Category.ToLower()}";
            }

            var client = new RestClient();
            var request = new RestRequest(url);
            var response = client.Execute(request);

            var fact = JsonConvert.DeserializeObject<Model.ChuckNorris>(response.Content);

            if (fact == null)
            {
                Log.Instance.Error("ChuckNorris deserialize object failed! JSON: {0}", fact);
                return string.Empty;
            }

            return fact.Value;
        }
    }
}
