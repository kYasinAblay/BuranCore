namespace Buran.Core.Library.OAuth
{
    public class ClientConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string UserInfoFields { get; set; }

        public ClientConfig(string clientId, string clientSecret, string scope, string userInfoFields = null)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scope = scope;
            UserInfoFields = userInfoFields;
        }
    }
}
