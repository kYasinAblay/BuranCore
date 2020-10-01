using System;

namespace Buran.Core.Library.Utils
{
    public static class GuidExtender
    {
        public static string ToCleanString(this Guid item)
        {
            return item.ToString().Replace("{", "").Replace("}", "");
        }
    }
}
