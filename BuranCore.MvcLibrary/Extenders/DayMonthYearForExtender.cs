using Buran.Core.MvcLibrary.ModelBinders.Model;
using Buran.Core.MvcLibrary.Resource;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Buran.Core.MvcLibrary.Extenders
{
    //[Obsolete]
    //public static class DayMonthYearForExtender
    //{
    //    public class ComboBoxItem
    //    {
    //        public int Id { get; set; }
    //        public string Text { get; set; }
    //    }

    //    private static List<ComboBoxItem> _months;
    //    public static List<ComboBoxItem> MonthList
    //    {
    //        get
    //        {
    //            if (_months == null)
    //            {
    //                _months = new List<ComboBoxItem>
    //                {
    //                    new ComboBoxItem { Id = 1, Text = UI.Month_1 },
    //                    new ComboBoxItem { Id = 2, Text = UI.Month_2 },
    //                    new ComboBoxItem { Id = 3, Text = UI.Month_3 },
    //                    new ComboBoxItem { Id = 4, Text = UI.Month_4 },
    //                    new ComboBoxItem { Id = 5, Text = UI.Month_5 },
    //                    new ComboBoxItem { Id = 6, Text = UI.Month_6 },
    //                    new ComboBoxItem { Id = 7, Text = UI.Month_7 },
    //                    new ComboBoxItem { Id = 8, Text = UI.Month_8 },
    //                    new ComboBoxItem { Id = 9, Text = UI.Month_9 },
    //                    new ComboBoxItem { Id = 10, Text = UI.Month_10 },
    //                    new ComboBoxItem { Id = 11, Text = UI.Month_11 },
    //                    new ComboBoxItem { Id = 12, Text = UI.Month_12 }
    //                };
    //            }
    //            return _months;
    //        }
    //    }


    //    private static List<int> _dayList;
    //    public static List<int> DayList
    //    {
    //        get
    //        {
    //            if (_dayList == null)
    //            {
    //                _dayList = new List<int>();
    //                for (var i = 1; i <= 31; i++)
    //                {
    //                    _dayList.Add(i);
    //                }
    //            }
    //            return _dayList;
    //        }
    //    }


    //    private static List<int> _yearList;
    //    public static List<int> YearList
    //    {
    //        get
    //        {
    //            if (_yearList == null)
    //            {
    //                _yearList = new List<int>();
    //                for (var i = 1900; i <= DateTime.Today.Year; i++)
    //                {
    //                    _yearList.Add(i);
    //                }
    //            }
    //            return _yearList;
    //        }
    //    }


    //    public static HtmlString DayMonthYearFor(this IHtmlHelper html, string name, DayMonthYear model)
    //    {
    //        return new HtmlString("<div class='divDMY'>" +
    //            html.DropDownList(name + ".Day", new SelectList(DayList, "Id", "Text", model.Day), new { @class = "DMY-day input-sm" }) +
    //            html.DropDownList(name + ".Month", new SelectList(MonthList, "Id", "Text", model.Month), new { @class = "DMY-month input-sm" }) +
    //            html.DropDownList(name + ".Year", new SelectList(YearList, "Id", "Text", model.Year), new { @class = "DMY-year input-sm" }) + "</div>"
    //            );
    //    }
    //}
}
