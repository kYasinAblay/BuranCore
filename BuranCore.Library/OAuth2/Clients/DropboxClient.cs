namespace Buran.Core.Library.OAuth.Clients
{
    public class DropboxClient : OAuth2Client
    {
        public DropboxClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://www.dropbox.com/1/oauth2/authorize";
            AccessTokenServiceEndpoint = "https://api.dropbox.com/1/oauth2/token";
            UserInfoServiceEndpoint = "https://api.dropbox.com/1/account/info";
            ProviderName = ClientTypes.Dropbox;
        }
    }
}