using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TPClient.Helpers;
using TPClient.Services;
using TPClient.Utilities;

namespace TPClient.APIs
{
    public class Rzhunemogu : IJokeAPI
    {
        public string Name => "РжуНеМогу.RU";

        public List<string> Categories => new List<string>
        {
            string.Empty,
            "Анекдоты",
            "Рассказы",
            "Стишки",
            "Афоризмы",
            "Цитаты",
            "Тосты",
            "Статусы",
            "Анекдот (+18)",
            "Рассказы (+18)",
            "Стишки (+18)",
            "Афоризмы (+18)",
            "Цитаты (+18)",
            "Тосты (+18)",
            "Статусы (+18)"
        };

        /// <summary>
        /// Get random fact.
        /// </summary>
        /// <returns></returns>
        public string GetJoke()
        {
            var url = "http://rzhunemogu.ru/Rand.aspx";

            var founded = false;
            var categoryId = 0;

            if (!string.IsNullOrEmpty(JokesManager.Instance.Category))
            {
                foreach (var category in Categories)
                {
                    if (category.Equals(JokesManager.Instance.Category))
                    {
                        founded = true;
                        break;
                    }

                    ++categoryId;
                }
            }

            if (founded)
            {
                url += $"?CType={categoryId}";
            }

            try
            {
                var reqGET = WebRequest.Create(url);
                var resp = reqGET.GetResponse();
                var stream = resp.GetResponseStream();
                var streamReader = new StreamReader(stream, Encoding.GetEncoding(1251));

                var joke = XmlExtension.Deserialize<Model.Rzhunemogu>(streamReader.ReadToEnd());

                if (joke == null)
                {
                    Log.Instance.Error("Rzhunemogu deserialize object failed! JSON: {0}", joke);
                    return string.Empty;
                }

                return joke.Content;
            }

            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }

            return string.Empty;
        }
    }
}
