using Buran.Core.Library.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Buran.Core.Library.Export
{

    public static class ExcelExtender
    {
        public static void ToCsvEXport<T>(this IEnumerable<T> list, string fileName)
        {
            var excelDoc = new StreamWriter(fileName, false, Encoding.UTF8);
            var rowCount = 0;
            var rowData = string.Empty;
            var type = typeof(T);
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                rowData += prop.Name + ";";
            }
            excelDoc.WriteLine(rowData);
            foreach (T x in list)
            {
                rowData = string.Empty;
                rowCount++;
                foreach (var prop in props)
                {
                    var value = Digger.GetObjectValue(x, prop.Name);
                    if (value == null)
                    {
                        rowData += ";";
                        continue;
                    }
                    switch (prop.PropertyType.ToString())
                    {
                        case "System.DateTime":
                            var xmlDate = (DateTime)value;
                            rowData += xmlDate.ToString("yyyy-MM-dd") + ";";
                            break;
                        case "System.String":
                        case "System.Boolean":
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            rowData += value + ";";
                            break;
                        case "System.Decimal":
                            rowData += value.ToString().Replace(",", ".");
                            break;
                        case "System.Double":
                            rowData += (decimal.Parse(value.ToString()).ToString("N2").Replace(",", "."));
                            break;
                        default:
                            rowData += ";";
                            break;
                    }
                }
                excelDoc.WriteLine(rowData);
            }
            excelDoc.Close();
        }

        public static void ToExcelExport<T>(this IEnumerable<T> list, string fileName, bool dateWithTime = false)
        {

            var excelDoc = new StreamWriter(fileName, false, Encoding.UTF8);
            const string startExcelXml = @"<xml version>
<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:x=""urn:schemas-microsoft-com:office:excel"" 
    xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet"">
<Styles>
    <Style ss:ID=""Default"" ss:Name=""Normal"">
        <Alignment ss:Vertical=""Bottom""/>
        <Borders/>
        <Font/>
        <Interior/>
        <NumberFormat/>
        <Protection/>
    </Style>
    <Style ss:ID=""BoldColumn"">
        <Font x:Family=""Swiss"" ss:Bold=""1""/>
    </Style>
    <Style ss:ID=""StringLiteral"">
        <NumberFormat ss:Format=""@""/>
    </Style>
    <Style ss:ID=""Decimal"">
        <NumberFormat ss:Format=""0.0000""/>
    </Style>
    <Style ss:ID=""Integer"">
        <NumberFormat ss:Format=""0""/>
    </Style>
    <Style ss:ID=""DateLiteral"">
        <NumberFormat ss:Format=""dd/mm/yy\\ h:mm;@""/>
    </Style>
</Styles>";
            const string endExcelXml = "</Workbook>";

            var sheetCount = 1;
            excelDoc.Write(startExcelXml);
            excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
            excelDoc.Write("<Table>");
            excelDoc.Write("<Row>");
            var type = typeof(T);
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                excelDoc.Write(prop.Name);
                excelDoc.Write("</Data></Cell>");
            }
            excelDoc.Write("</Row>");
            foreach (T x in list)
            {
                excelDoc.Write("<Row>");
                foreach (var prop in props)
                {
                    var value = Digger.GetObjectValue(x, prop.Name);
                    if (value == null)
                    {
                        excelDoc.Write("<Cell></Cell>");
                        continue;
                    }
                    switch (prop.PropertyType.ToString())
                    {
                        case "System.String":
                            var xmLstring = value;
                            xmLstring = xmLstring.Trim();
                            xmLstring = xmLstring.Replace("&", "&");
                            xmLstring = xmLstring.Replace(">", ">");
                            xmLstring = xmLstring.Replace("<", "<");
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
                            excelDoc.Write(xmLstring);
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.DateTime":
                            var xmlDate = (DateTime)value;
                            if (dateWithTime)
                            {
                                var xmlDatetoString = xmlDate.Date.ToString("yyyy-MM-dd");
                                excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\"><Data ss:Type=\"DateTime\">");
                                excelDoc.Write(xmlDatetoString);
                                excelDoc.Write("</Data></Cell>");
                            }
                            else
                            {
                                var xmlDatetoString = string.Format("{0}-{1}-{2}T{3}:{4}:{5}.000", xmlDate.Year,
                                                                    (xmlDate.Month < 10
                                                                         ? "0" + xmlDate.Month
                                                                         : xmlDate.Month.ToString()),
                                                                    (xmlDate.Day < 10
                                                                         ? "0" + xmlDate.Day
                                                                         : xmlDate.Day.ToString()),
                                                                    (xmlDate.Hour < 10
                                                                         ? "0" + xmlDate.Hour
                                                                         : xmlDate.Hour.ToString()),
                                                                    (xmlDate.Minute < 10
                                                                         ? "0" + xmlDate.Minute
                                                                         : xmlDate.Minute.ToString()),
                                                                    (xmlDate.Second < 10
                                                                         ? "0" + xmlDate.Second
                                                                         : xmlDate.Second.ToString()));
                                excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\"><Data ss:Type=\"DateTime\">");
                                excelDoc.Write(xmlDatetoString);
                                excelDoc.Write("</Data></Cell>");
                            }
                            break;
                        case "System.Nullable`1[System.DateTime]":
                            var xmlDate2 = (DateTime?)value;
                            if (dateWithTime)
                            {
                                if (xmlDate2.HasValue)
                                {
                                    var xmlDate3 = xmlDate2.Value.Date;
                                    var xmlDatetoString = xmlDate3.ToString("yyyy-MM-dd");
                                    excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\"><Data ss:Type=\"DateTime\">");
                                    excelDoc.Write(xmlDatetoString);
                                    excelDoc.Write("</Data></Cell>");
                                }
                                else
                                {
                                    excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
                                    excelDoc.Write("");
                                    excelDoc.Write("</Data></Cell>");
                                }
                            }
                            else
                            {
                                if (xmlDate2.HasValue)
                                {
                                    var xmlDatetoString = string.Format("{0}-{1}-{2}T{3}:{4}:{5}.000", xmlDate2.Value.Year,
                                        (xmlDate2.Value.Month < 10
                                            ? "0" + xmlDate2.Value.Month
                                            : xmlDate2.Value.Month.ToString()),
                                        (xmlDate2.Value.Day < 10
                                            ? "0" + xmlDate2.Value.Day
                                            : xmlDate2.Value.Day.ToString()),
                                        (xmlDate2.Value.Hour < 10
                                            ? "0" + xmlDate2.Value.Hour
                                            : xmlDate2.Value.Hour.ToString()),
                                        (xmlDate2.Value.Minute < 10
                                            ? "0" + xmlDate2.Value.Minute
                                            : xmlDate2.Value.Minute.ToString()),
                                        (xmlDate2.Value.Second < 10
                                            ? "0" + xmlDate2.Value.Second
                                            : xmlDate2.Value.Second.ToString()));
                                    excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\"><Data ss:Type=\"DateTime\">");
                                    excelDoc.Write(xmlDatetoString);
                                    excelDoc.Write("</Data></Cell>");
                                }
                                else
                                {
                                    excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
                                    excelDoc.Write("");
                                    excelDoc.Write("</Data></Cell>");
                                }
                            }
                            break;
                        case "System.Boolean":
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
                            excelDoc.Write(value.ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            excelDoc.Write("<Cell ss:StyleID=\"Integer\"><Data ss:Type=\"Number\">");
                            excelDoc.Write(value.ToString());
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Decimal":
                            excelDoc.Write("<Cell ss:StyleID=\"Decimal\"><Data ss:Type=\"Number\">");
                            excelDoc.Write(value.ToString().Replace(",", "."));
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.Double":
                            excelDoc.Write("<Cell ss:StyleID=\"Decimal\"><Data ss:Type=\"Number\">");
                            excelDoc.Write((decimal.Parse(value.ToString()).ToString("N2").Replace(",", ".")));
                            excelDoc.Write("</Data></Cell>");
                            break;
                        case "System.DBNull":
                            excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\"><Data ss:Type=\"String\">");
                            excelDoc.Write("");
                            excelDoc.Write("</Data></Cell>");
                            break;
                        default:
                            throw (new Exception(prop.PropertyType.ToString() + " not handled."));
                    }
                }
                excelDoc.Write("</Row>");
            }
            excelDoc.Write("</Table>");
            excelDoc.Write("</Worksheet>");
            excelDoc.Write(endExcelXml);
            excelDoc.Close();
            excelDoc.Dispose();
        }
    }
}
