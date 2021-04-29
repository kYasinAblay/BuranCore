using System;
using System.Collections.Generic;

namespace Buran.Core.Library.OAuth.Clients
{
    class LinkedInClient : OAuth2Client
    {
        public LinkedInClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://www.linkedin.com/uas/oauth2/authorization";
            AccessTokenServiceEndpoint = "https://www.linkedin.com/uas/oauth2/accessToken";
            UserInfoServiceEndpoint = "https://api.linkedin.com/v1/people/~:(id,first-name,last-name,picture-url,email-address)";
            ProviderName = ClientTypes.Linkedin;
        }

        public override string GetAccessCodeUrl(AuthenticationResponseType responseType, string redirectUrl, List<string> state, Dictionary<string, string> extraData)
        {
            state.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
            return base.GetAccessCodeUrl(responseType, redirectUrl, state, extraData);
        }

        protected override void BeforeGetUserInfo(string accessToken, Dictionary<string, string> data, Dictionary<string, string> header)
        {
            header.Add("x-li-format", "json");
            base.BeforeGetUserInfo(accessToken, data, header);
        }
    }
}
