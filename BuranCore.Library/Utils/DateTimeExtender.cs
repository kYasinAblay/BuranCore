using System;
using System.Globalization;
using System.Text;

namespace Buran.Core.Library.Utils
{
    public static class DateTimeExtender
    {
        public static long UnixTimeNow(this DateTime obj)
        {
            TimeSpan _TimeSpan = obj - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(_TimeSpan.TotalSeconds);
        }

        public static string ToUtcDateTimeString(this DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            int hour = date.Hour;
            int minute = date.Minute;

            StringBuilder sb = new StringBuilder();
            sb.Append(year.ToString());
            sb.Append("-");
            sb.Append(month.ToString().PadLeft(2, '0'));
            sb.Append("-");
            sb.Append(day.ToString().PadLeft(2, '0'));
            sb.Append("T");
            sb.Append(hour.ToString().PadLeft(2, '0'));
            sb.Append(":");
            sb.Append(minute.ToString().PadLeft(2, '0'));
            sb.Append("+03:00");
            return sb.ToString();
        }


        public class WeekInfo
        {
            public int WeekNumber { get; set; }
            public DateTime BeginDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        public static WeekInfo GetWeekInfo(this DateTime date)
        {
            var result = new WeekInfo
            {
                WeekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Friday)
            };
            var dw = (int)date.DayOfWeek;
            dw += 2;
            if (dw > 6)
            {
                dw -= 7;
            }

            result.BeginDate = date.AddDays(-1 * dw);
            result.EndDate = result.BeginDate.AddDays(6);
            return result;
        }
    }
}
