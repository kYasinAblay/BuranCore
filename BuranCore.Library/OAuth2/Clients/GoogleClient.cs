namespace Buran.Core.Library.OAuth.Clients
{
    public class GoogleClient : OAuth2Client
    {
        public GoogleClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://accounts.google.com/o/oauth2/auth";
            AccessTokenServiceEndpoint = "https://accounts.google.com/o/oauth2/token";
            UserInfoServiceEndpoint = "https://www.googleapis.com/oauth2/v1/userinfo";
            ProviderName = ClientTypes.Google;
            DefaultScope = "https://www.googleapis.com/auth/plus.me email ";
        }
    }
}