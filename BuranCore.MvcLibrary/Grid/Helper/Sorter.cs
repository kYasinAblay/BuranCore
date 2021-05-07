using Buran.Core.Library.Utils;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Buran.Core.MvcLibrary.Grid.Helper
{
    class SorterInfo
    {
        public string Keyword { get; set; }
        public string Direction { get; set; }
    }

    class Sorter
    {
        public List<SorterInfo> List { get; set; }
        public string CleanQueryString { get; set; }

        public Sorter(List<KeyValuePair<string, string>> query, string sortKeyword, string pagerAndShortAction)
        {
            List = new List<SorterInfo>();
            var sortValueItem = query.FirstOrDefault(d => d.Key == sortKeyword);
            if (!sortValueItem.Value.IsEmpty())
            {
                var sortFields = sortValueItem.Value.Split(',');
                foreach (var field in sortFields)
                {
                    var s = field.Split(' ');
                    if (s.Length == 2)
                        List.Add(new SorterInfo { Direction = s[1], Keyword = s[0] });
                }
            }

            var qc = new List<KeyValuePair<string, string>>(query);
            qc.RemoveAll(d => d.Key == sortKeyword);
            var qb = new QueryBuilder(qc);
            var qq = qb.ToQueryString();
            CleanQueryString = qq.ToUriComponent().Replace("?", "");

            var ci = pagerAndShortAction.Split('?');
            if (ci.Length > 1)
                CleanQueryString = CleanQueryString.Replace(ci[1], "");
            if (CleanQueryString == "?")
                CleanQueryString = "";
            if (CleanQueryString.StartsWith("&"))
                CleanQueryString = CleanQueryString.Substring(1);
        }

        public string GetSortImg(string fieldName)
        {
            var sortingData = List.Where(d => d.Keyword == fieldName);
            if (sortingData.Any())
            {
                var sortInfo = sortingData.First();
                return sortInfo.Direction == "ASC"
                    ? "<img class='tableImg' src='" + @"/Content/admin/plugins/MvcGrid/images/sort-asc.png" + "' />"
                    : sortInfo.Direction == "DESC"
                            ? "<img class='tableImg' src='" + @"/Content/admin/plugins/MvcGrid/images/sort-desc.png" + "' />"
                            : string.Empty;
            }
            return string.Empty;
        }

        public string GetSortImg4(string fieldName)
        {
            var sortingData = List.Where(d => d.Keyword == fieldName);
            if (sortingData.Any())
            {
                var sortInfo = sortingData.First();
                return sortInfo.Direction == "ASC"
                    ? "<img class='tableImg' src='" + @"/assets/zero/mvcgrid/images/sort-asc.png" + "' />"
                    : sortInfo.Direction == "DESC"
                            ? "<img class='tableImg' src='" + @"/assets/zero/mvcgrid/images/sort-desc.png" + "' />"
                            : string.Empty;
            }
            return string.Empty;
        }

        public string GetSortParam(string fieldName, string defaultFieldName, string defaultDirection)
        {
            var str = string.Empty;
            var exits = false;
            foreach (var sortInfo in List)
            {
                if (!str.IsEmpty())
                    str += ",";
                if (sortInfo.Keyword == fieldName)
                {
                    exits = true;
                    str += sortInfo.Direction == "ASC"
                           ? sortInfo.Keyword + " DESC"
                           : sortInfo.Keyword + " ASC";
                }
                else
                    str += sortInfo.Keyword + " " + sortInfo.Direction;
            }
            if (!exits)
            {
                //if (!string.IsNullOrWhiteSpace(str))
                //    str += ",";
                str = fieldName + " ASC";
            }
            return str;
        }
    }
}
