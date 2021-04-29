namespace Buran.Core.Library.OAuth.Clients
{
    public class WindowsLiveClient : OAuth2Client
    {
        public WindowsLiveClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://login.live.com/oauth20_authorize.srf";
            AccessTokenServiceEndpoint = "https://login.live.com/oauth20_token.srf";
            UserInfoServiceEndpoint = "https://apis.live.net/v5.0/me";
            ProviderName = ClientTypes.WindowsLive;
            DefaultScope = "wl.basic wl.signin wl.emails wl.share";
        }
    }
}