using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Linq;

namespace Buran.Core.MvcLibrary.Utils
{
    public static class QueryStringExtender
    {
        public static List<KeyValuePair<string, string>> ToStringList(this QueryString _query)
        {
            var _queryDictionary = QueryHelpers.ParseQuery(_query.ToString());
            var _queryItems = _queryDictionary
                .SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value))
                .ToList();
            return _queryItems;
        }
    }
}
