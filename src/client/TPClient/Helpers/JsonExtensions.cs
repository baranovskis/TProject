using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TPClient.Helpers
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Convert object to Entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TEntity Convert<TEntity>(this object obj)
        {
            var sObj = obj.ToString();

            if (!(obj is string || obj is JToken))
                JsonConvert.SerializeObject(obj);

            return JsonConvert.DeserializeObject<TEntity>(sObj);
        }
    }
}
