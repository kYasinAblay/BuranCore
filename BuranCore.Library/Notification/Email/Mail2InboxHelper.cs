using Buran.Core.Library.Http;
using Buran.Core.Library.Utils;
using System.Collections.Generic;

namespace Buran.Core.Library.Notification.Email
{
    public class ResultModel
    {
        public int Id { get; set; }
        public string Err { get; set; }
    }

    public class TokenResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class SendPostModel
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Reply { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }

    public class Mail2InboxHelper
    {
        public TokenResult GetToken(string userName, string password)
        {
            var client = new WebRequest2();

            var data = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"username", userName},
                {"password", password.ToBase64()}
            };

            var tokenResult = client.PostForm("https://api.mail2inbox.com/token", data);
            return tokenResult.ParseJson<TokenResult>();
        }

        public ResultModel AddMember(string firstName, string lastName, string email, int listId, TokenResult token)
        {
            var client = new WebRequest2();
            var data = new Dictionary<string, string>
            {
                {"Email", email},
                {"FirstName", firstName},
                {"LastName", lastName},
                {"ListId", listId.ToString()}
            };
            var resultData = client.PostForm("https://api.mail2inbox.com/api/Member", data, token.access_token);
            return resultData.ParseJson<ResultModel>();
        }

        public ResultModel SendMail(SendPostModel msg, TokenResult token)
        {
            var client = new WebRequest2();
            var data = new Dictionary<string, string>
            {
                {"From", msg.From},
                {"To", msg.To},
                {"Reply", msg.Reply},
                {"Cc", msg.Cc},
                {"Bcc", msg.Bcc},
                {"Subject", msg.Subject},
                {"Body", msg.Body}
            };
            var resultData = client.PostForm("https://api.mail2inbox.com/api/send", data, token.access_token);
            return resultData.ParseJson<ResultModel>();
        }
    }
}
