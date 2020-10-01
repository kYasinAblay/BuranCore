using Buran.Core.Library.Http;
using Buran.Core.Library.Utils;
using System.Collections.Generic;

namespace Buran.Core.Library.Notification.Email
{
    public class RecaptchaResultModel
    {
        public bool success { get; set; }
    }

    public class RecaptchaChecker
    {
        public RecaptchaResultModel CheckIt(string secret, string code)
        {
            var client = new WebRequest2();
            var data = new Dictionary<string, string>
            {
                {"secret", secret},
                {"response", code}
            };

            var tokenResult = client.PostForm("https://www.google.com/recaptcha/api/siteverify", data);
            return tokenResult.ParseJson<RecaptchaResultModel>();
        }
    }
}
