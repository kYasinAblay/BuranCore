namespace Buran.Core.Library.OAuth.Clients
{
    public class YandexClient : OAuth2Client
    {
        public YandexClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://oauth.yandex.com/authorize";
            AccessTokenServiceEndpoint = "https://oauth.yandex.com/token";
            UserInfoServiceEndpoint = "https://login.yandex.com/info";
            ProviderName = ClientTypes.Yandex;
        }
    }
}