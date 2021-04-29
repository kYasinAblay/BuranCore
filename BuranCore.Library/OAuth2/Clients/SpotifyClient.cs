using System;
using System.Collections.Generic;

namespace Buran.Core.Library.OAuth.Clients
{
    public class SpotifyClient : OAuth2Client
    {
        public SpotifyClient(ClientConfig configuration)
            : base(configuration)
        {
            AccessCodeServiceEndpoint = "https://accounts.spotify.com/authorize";
            AccessTokenServiceEndpoint = "https://accounts.spotify.com/api/token";
            UserInfoServiceEndpoint = "https://api.spotify.com/v1/me";
            ProviderName = ClientTypes.Spotify;
            DefaultScope = "playlist-read-private user-read-private user-read-email";
            AuthenticationType = AuthenticationTypes.Header;
        }

        public override string GetAccessCodeUrl(AuthenticationResponseType responseType, string redirectUrl, List<string> state, Dictionary<string, string> extraData)
        {
            state.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
            return base.GetAccessCodeUrl(responseType, redirectUrl, state, extraData);
        }
    }
}