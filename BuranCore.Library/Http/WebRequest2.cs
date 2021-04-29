using Buran.Core.Library.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Buran.Core.Library.Http
{
    public class WebRequest2
    {
        private HttpClient GetClient(string authorizationToken = null, string authorizationSchema = "Bearer")
        {
            var client = new HttpClient();
            if (!authorizationToken.IsEmpty())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorizationSchema, authorizationToken);
            }

            return client;
        }

        private HttpClient GetBasicClient(string userName, string password)
        {
            var token = $"{userName}:{password}".ToBase64();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            return client;
        }



        public string PostData(string url, Dictionary<string, string> data,
          string authorizationToken = null, string authorizationSchema = "Bearer")
        {
            var client = GetClient(authorizationToken, authorizationSchema);
            var content = new StringContent(data.ToEncode(), Encoding.UTF8);
            var response = client.PostAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string PostData2(string url, string postData)
        {
            var byteArray = Encoding.UTF8.GetBytes(postData);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.KeepAlive = false;
            request.ContentLength = byteArray.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //try
            //{
            var response = request.GetResponse();
            var responseCode = ((HttpWebResponse)response).StatusCode;
            var reader = new StreamReader(response.GetResponseStream());
            var responseLine = reader.ReadToEnd();
            reader.Close();
            return responseLine;
            //}
            //catch (Exception ex)
            //{
            //    return LogUtil.Logger.GetErrorMessage(ex);
            //}
            //return "error";
        }



        public string PostForm(string url, Dictionary<string, string> data,
            string authorizationToken = null, string authorizationSchema = "Bearer")
        {
            var client = GetClient(authorizationToken, authorizationSchema);
            var content = new FormUrlEncodedContent(data);
            var response = client.PostAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string PostJson(string url, string data,
           string authorizationToken = null, string authorizationSchema = "Bearer")
        {
            var client = GetClient(authorizationToken, authorizationSchema);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = client.PostAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string PostJsonBasicAuth(string url, dynamic data, string userName, string password = "")
        {
            var client = GetBasicClient(userName, password);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = client.PostAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }



        public string Delete(string url, string authorizationToken = null, string authorizationSchema = "Bearer")
        {
            var client = GetClient(authorizationToken, authorizationSchema);
            var response = client.DeleteAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }



        public string PutJson(string url, string data,
           string authorizationToken = null, string authorizationSchema = "Bearer")
        {
            var client = GetClient(authorizationToken, authorizationSchema);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = client.PutAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string PutJsonBasicAuth(string url, string data, string userName, string password = "")
        {
            var client = GetBasicClient(userName, password);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = client.PutAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }



        public string PutFile(string url, byte[] data,
           string authorizationToken = null, string authorizationSchema = "Bearer")
        {
            var client = GetClient(authorizationToken, authorizationSchema);

            var content = new ByteArrayContent(data);
            var response = client.PutAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string PutFile(string url, byte[] data, long from, long to, long length,
            string authorizationToken = null, string authorizationSchema = "Bearer")
        {
            if (to > length)
                to = length - 1;

            var client = GetClient(authorizationToken, authorizationSchema);
            var ht = new HttpRequestMessage(HttpMethod.Put, url) { Content = new ByteArrayContent(data) };
            ht.Content.Headers.ContentRange = new ContentRangeHeaderValue(from, to, length);
            var response = client.SendAsync(ht).Result;
            return response.Content.ReadAsStringAsync().Result;
        }



        public string GetUrl(string url, Dictionary<string, string> data, Dictionary<string, string> headerData = null,
                string authorizationToken = null, string authorizationSchema = "Bearer")
        {
            var client = GetClient(authorizationToken, authorizationSchema);
            if (headerData != null && headerData.Count > 0)
            {
                foreach (var hd in headerData)
                    client.DefaultRequestHeaders.Add(hd.Key, hd.Value);
            }

            var getUrl = url;
            if (data != null && data.Count > 0)
                getUrl += "?" + data.ToEncode();

            var response = client.GetAsync(getUrl).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }



        public string GetJsonBasicAuth(string url, string userName, string password = "")
        {
            var client = GetBasicClient(userName, password);
            var response = client.GetAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
