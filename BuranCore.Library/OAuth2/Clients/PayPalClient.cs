using Buran.Core.Library.Utils;
using System;
using System.Collections.Generic;

namespace Buran.Core.Library.OAuth.Clients
{
    class PayPalClient : OAuth2Client
    {
        public PayPalClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://www.paypal.com/webapps/auth/protocol/openidconnect/v1/authorize";
            AccessTokenServiceEndpoint = "https://api.paypal.com/v1/identity/openidconnect/tokenservice";
            UserInfoServiceEndpoint = "https://api.paypal.com/v1/identity/openidconnect/userinfo";
            ProviderName = ClientTypes.PayPal;
            ResponseType = ResponseTypes.Querystring;
        }

        public override string GetAccessCodeUrl(AuthenticationResponseType responseType, string redirectUrl, List<string> state, Dictionary<string, string> extraData)
        {
            var k = DateTime.Now.ToString("yyyyMMddHHmmssff").ToBase64().Substring(0, 16);
            var nonce = DateTime.Now.ToString("yyyyMMddHHmmss") + k;
            extraData.Add("nonce", nonce);
            return base.GetAccessCodeUrl(responseType, redirectUrl, state, extraData);
        }

        protected override void BeforeGetUserInfo(string accessToken, Dictionary<string, string> data, Dictionary<string, string> header)
        {
            data.Add("schema", "openid");
            base.BeforeGetUserInfo(accessToken, data, header);
        }
    }
}
