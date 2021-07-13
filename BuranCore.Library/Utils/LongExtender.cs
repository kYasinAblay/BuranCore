using System;

namespace Buran.Core.Library.Utils
{
    public static class LongExtender
    {
        public static string ToKb(this long value)
        {
            return (value > 1024
                        ? (value > 1024 * 1024
                               ? (value > 1024 * 1024 * 1024
                                      ? Math.Round(((double)value) / (1024 * 1024 * 1024), 0) + " GB"
                                      : Math.Round(((double)value) / (1024 * 1024), 0) + " MB")
                               : Math.Round(((double)value) / (1024), 0) + " KB")
                        : value + " B");
        }
        public static string ToKb(this int value)
        {
            return (value > 1024
                        ? (value > 1024 * 1024
                               ? (value > 1024 * 1024 * 1024
                                      ? Math.Round(((double)value) / (1024 * 1024 * 1024), 0) + " GB"
                                      : Math.Round(((double)value) / (1024 * 1024), 0) + " MB")
                               : Math.Round(((double)value) / (1024), 0) + " KB")
                        : value + " B");
        }
    }
}
