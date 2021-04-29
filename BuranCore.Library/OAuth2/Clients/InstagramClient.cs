namespace Buran.Core.Library.OAuth.Clients
{
    public class InstagramClient : OAuth2Client
    {
        public InstagramClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://api.instagram.com/oauth/authorize";
            AccessTokenServiceEndpoint = "https://api.instagram.com/oauth/access_token";
            UserInfoServiceEndpoint = "https://api.instagram.com/oauth/access_token";
            ProviderName = ClientTypes.Instagram;
        }
    }
}