using Buran.Core.Library.OAuth.Clients;
using System.Collections.Generic;
using System.Linq;

namespace Buran.Core.Library.OAuth
{
    public static class OAuth2
    {
        private static List<OAuth2Client> Clients { get; set; }

        static OAuth2()
        {
            Clients = new List<OAuth2Client>();
        }

        public static List<ClientTypes> GetClientList()
        {
            return Clients.Select(item => item.ProviderName).ToList();
        }

        public static void ClearClientList()
        {
            Clients.Clear();
        }

        public static void Register(ClientTypes type, ClientConfig config)
        {
            switch (type)
            {
                case ClientTypes.Facebook:
                    Clients.Add(new FacebookClient(config));
                    break;
                case ClientTypes.Deezer:
                    Clients.Add(new DeezerClient(config));
                    break;
                case ClientTypes.Dropbox:
                    Clients.Add(new DropboxClient(config));
                    break;
                case ClientTypes.Google:
                    Clients.Add(new GoogleClient(config));
                    break;
                case ClientTypes.Instagram:
                    Clients.Add(new InstagramClient(config));
                    break;
                case ClientTypes.Linkedin:
                    Clients.Add(new LinkedInClient(config));
                    break;
                case ClientTypes.PayPal:
                    Clients.Add(new PayPalClient(config));
                    break;
                case ClientTypes.Spotify:
                    Clients.Add(new SpotifyClient(config));
                    break;
                case ClientTypes.WindowsLive:
                    Clients.Add(new WindowsLiveClient(config));
                    break;
                case ClientTypes.Yandex:
                    Clients.Add(new YandexClient(config));
                    break;
            }
        }

        public static OAuth2Client GetClient(ClientTypes provider)
        {
            var client = Clients.FirstOrDefault(d => d.ProviderName == provider);
            return client;
        }

        // 1
        public static string RequestAuthenticationCodeUrl(ClientTypes provider, string returnUrl, List<string> state)
        {
            var client = GetClient(provider);
            var url = client.GetAccessCodeUrl(OAuth2Client.AuthenticationResponseType.Code, returnUrl, state, new Dictionary<string, string>());
            return url;
        }

        // 1
        public static string RequestAuthenticationTokenUrl(ClientTypes provider, string returnUrl)
        {
            var client = GetClient(provider);
            var url = client.GetAccessCodeUrl(OAuth2Client.AuthenticationResponseType.Token, returnUrl, null, new Dictionary<string, string>());
            return url;
        }

        //2
        public static AuthenticationToken GetAuthenticationToken(ClientTypes provider, string returnUrl, string code)
        {
            var client = GetClient(provider);
            return client.GetAuthenticationToken(code, returnUrl);
        }

        // 2.5
        public static AuthenticationToken GetRefreshToken(ClientTypes provider, string refreshToken)
        {
            var client = GetClient(provider);
            return client.GetRefreshToken(refreshToken);
        }

        // 3
        public static T GetUserInfo<T>(ClientTypes provider, string accessToken, string tokenType = null)
            where T : class, new()
        {
            var client = GetClient(provider);
            return client.GetUserInfo<T>(accessToken, tokenType);
        }

        public static dynamic GetUserInfo(ClientTypes provider, string accessToken, string tokenType = null)
        {
            var client = GetClient(provider);
            return client.GetUserInfo(accessToken, tokenType);
        }
    }
}
