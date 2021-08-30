using Buran.Core.Library.Utils;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Buran.Core.Library.Notification.Push
{
    public class FirebasePush
    {
        private const string Url = "https://fcm.googleapis.com/fcm/send";
        private readonly string _key;

        public FirebasePush(string key)
        {
            _key = key;
        }

        public string PushToDevice(string to, string title, string body, bool isContent = false, string data = null)
        {
            if (data.IsEmpty())
                data = "";
            var content = isContent ? @"""content_available"": true," : "";
            var postData = $@"
                {{""to"": ""{to}"", 
                {content}
                {data}
                ""notification"": {{""title"": ""{title}"", ""body"": ""{body}""}}
                }}";
            return Send(postData);
        }

        public string PushDataToDevice(string to, string data)
        {
            if (data.IsEmpty())
                data = "";
            var content = @"""content_available"": true,";
            var postData = $@"
                {{""to"": ""{to}"", 
                {content}
                {data}
                }}";
            return Send(postData);
        }

        public string PushToTopic(string topic, string title, string body, bool isContent = false, string data = null)
        {
            if (data.IsEmpty())
                data = "";
            var content = isContent ? @"""content_available"": true," : "";
            var postData = $@"
                {{""condition"": ""'{topic}' in topics"", 
                {content}
                {data}
                ""notification"": {{""title"": ""{title}"", ""body"": ""{body}""}}
                }}";
            return Send(postData);
        }

        public string PushDataToTopic(string topic, string data)
        {
            if (data.IsEmpty())
                data = "";
            var content = @"""content_available"": true,";
            var postData = $@"
                {{""condition"": ""'{topic}' in topics"", 
                {content}
                {data}
                }}";
            return Send(postData);
        }

        public string Send(string postData)
        {
            string postDataContentType = "application/json";
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            var byteArray = Encoding.UTF8.GetBytes(postData);
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.KeepAlive = false;
            request.ContentType = postDataContentType;
            request.Headers.Add($"Authorization: key={_key}");
            request.ContentLength = byteArray.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            try
            {
                var response = request.GetResponse();
                var responseCode = ((HttpWebResponse)response).StatusCode;
                if (responseCode.Equals(HttpStatusCode.Unauthorized) || responseCode.Equals(HttpStatusCode.Forbidden))
                {
                    //var text = "Unauthorized - need new token";
                }
                else if (!responseCode.Equals(HttpStatusCode.OK))
                {
                    //var text = "Response from web service isn't OK";
                }
                var reader = new StreamReader(response.GetResponseStream());
                var responseLine = reader.ReadToEnd();
                reader.Close();
                return responseLine;
            }
            catch (Exception)
            {
            }
            return "error";
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}