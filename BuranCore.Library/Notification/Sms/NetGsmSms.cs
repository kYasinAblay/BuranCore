using Buran.Core.Library.LogUtil;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Buran.Core.Library.Notification.Sms
{
    public class NetgsmResponse
    {
        public int Code { get; set; }
        public string Id { get; set; }
        public string Err { get; set; }
    }

    public class NetGsm
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly string _from;

        public NetGsm(string userName, string password, string from)
        {
            _userName = userName;
            _password = password;
            _from = from;
        }

        public NetgsmResponse SendSms(string toPhone, string msg)
        {
            try
            {
                var apiAdres = "https://api.netgsm.com.tr/sms/send/xml";
                var xmlData = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<mainbody>
    <header>
        <company dil=""TR"">Netgsm</company>
        <usercode>{_userName}</usercode>
        <password>{_password}</password>
        <type>1:n</type>
        <msgheader>{_from}</msgheader>
    </header>
    <body>
        <msg><![CDATA[{msg}]]></msg>
        <no>{toPhone}</no>
    </body>
</mainbody>";

                var request = WebRequest.Create(apiAdres);
                request.Method = "POST";
                var byteArray = Encoding.UTF8.GetBytes(xmlData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                var dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                var response = request.GetResponse();
                dataStream = response.GetResponseStream();
                var reader = new StreamReader(dataStream);
                var responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                var ss = responseFromServer.Split(' ');
                var res = new NetgsmResponse();
                if (ss.Length <= 2)
                {
                    int.TryParse(ss[0], out int i);

                    res.Code = i;
                    if (res.Code == 0)
                    {
                        res.Id = ss[1];
                    }
                    else if (res.Code == 20)
                    {
                        res.Err = "Mesaj çok uzun";
                    }
                    else if (res.Code == 30)
                    {
                        res.Err = "IP kısıtlı";
                    }
                    else if (res.Code == 40)
                    {
                        res.Err = "Gönderici adı sorunu";
                    }
                    else if (res.Code == 70)
                    {
                        res.Err = "Genel hata";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return new NetgsmResponse { Err = Logger.GetErrorMessage(ex) };
            }
        }
    }
}
