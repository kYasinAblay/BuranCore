using System.Collections.Generic;

namespace Buran.Core.Library.OAuth.Clients
{
    public class FacebookClient : OAuth2Client
    {
        public FacebookClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://www.facebook.com/dialog/oauth";
            AccessTokenServiceEndpoint = "https://graph.facebook.com/oauth/access_token";
            UserInfoServiceEndpoint = "https://graph.facebook.com/me";
            ProviderName = ClientTypes.Facebook;
            ResponseType = ResponseTypes.Json;
            DefaultScope = "email,public_profile,user_friends";
        }

        protected override void BeforeGetUserInfo(string accessToken, Dictionary<string, string> data, Dictionary<string, string> header)
        {
            data.Add("fields", "id,first_name,last_name,email,picture");
            base.BeforeGetUserInfo(accessToken, data, header);
        }
    }
}