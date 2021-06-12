using System.Text.Json;

namespace Buran.Core.Library.Utils
{
    public static class JsonStringExtender
    {
        public static string ToJson<T>(this T json) where T : class, new()
        {
            return JsonSerializer.Serialize(json);
        }

        public static T ParseJson<T>(this string json) where T : class, new()
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
