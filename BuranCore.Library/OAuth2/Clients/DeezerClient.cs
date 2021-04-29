using System;
using System.Collections.Generic;

namespace Buran.Core.Library.OAuth.Clients
{
    public class DeezerClient : OAuth2Client
    {
        public DeezerClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://connect.deezer.com/oauth/auth.php";
            AccessTokenServiceEndpoint = "https://connect.deezer.com/oauth/access_token.php";
            UserInfoServiceEndpoint = "https://api.deezer.com/user/me";
            ProviderName = ClientTypes.Deezer;
            ResponseType = ResponseTypes.Querystring;
            ClientIdKeyword = "app_id";
            ScopeKeyword = "perms";
        }

        public override string GetAccessCodeUrl(AuthenticationResponseType responseType, string redirectUrl, List<string> state, Dictionary<string, string> extraData)
        {
            state.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
            return base.GetAccessCodeUrl(responseType, redirectUrl, state, extraData);
        }
    }
}