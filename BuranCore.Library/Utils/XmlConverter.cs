using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace Buran.Core.Library.Utils
{
    public static class XmlConverter
    {
        public static T Deserialize<T>(string input) where T : class
        {
            var ser = new XmlSerializer(typeof(T));
            using (var sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static string Serialize<T>(T ObjectToSerialize)
        {
            var xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }

        public static string ConvertToHtmlFromBase64(string xslDataBase64, string xmlData)
        {
            byte[] data = Convert.FromBase64String(xslDataBase64);
            string decodedXslt = System.Text.Encoding.UTF8.GetString(data);
            return ConvertToHtml(decodedXslt, xmlData);
        }

        public static string ConvertToHtml(string xslData, string xmlData)
        {
            using (var srt = new StringReader(xslData))
            {
                using (var sri = new StringReader(xmlData))
                {
                    using (var xrt = XmlReader.Create(srt))
                    {
                        using (var xri = XmlReader.Create(sri))
                        {
                            var xslt = new XslCompiledTransform();
                            xslt.Load(xrt);
                            using (var sw = new StringWriter())
                            {
                                using (var xwo = XmlWriter.Create(sw, xslt.OutputSettings))
                                {
                                    xslt.Transform(xri, xwo);
                                    return sw.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
