using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Buran.Core.Library.Utils
{
    public static class ListExtender
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string GetMd5Hash<T>(this List<T> text)
        {
            string json = JsonConvert.SerializeObject(text);
            return json.GetMd5Hash();
        }
    }
}
