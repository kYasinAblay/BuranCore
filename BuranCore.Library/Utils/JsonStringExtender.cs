using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Buran.Core.Library.Utils
{
    public static class JsonStringExtender
    {
        public static string ToJson2<T>(this T json) where T : class, new()
        {
            return System.Text.Json.JsonSerializer.Serialize(json);
        }

        public static T ParseJson2<T>(this string json) where T : class, new()
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public static string ToJson<T>(this T json) where T : class, new()
        {
            return JsonConvert.SerializeObject(json);
        }

        public static T ParseJson<T>(this string json) where T : class, new()
        {
            var jobject = JObject.Parse(json);
            return JsonConvert.DeserializeObject<T>(jobject.ToString());
        }

        public static T[] ParseJsonArray<T>(this string json) where T : class, new()
        {
            var jobject = JArray.Parse(json);
            return JsonConvert.DeserializeObject<T[]>(jobject.ToString());
        }
    }
}