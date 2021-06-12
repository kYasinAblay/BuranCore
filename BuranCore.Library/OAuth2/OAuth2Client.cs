using System;
using System.Collections.Generic;
using System.Web;
using Buran.Core.Library.Utils;
using Buran.Core.Library.Http;
using Buran.Core.Library.LogUtil;

namespace Buran.Core.Library.OAuth
{
    public abstract class OAuth2Client
    {
        public enum AuthenticationResponseType
        {
            Code,
            Token
        }

        protected enum ResponseTypes
        {
            Json,
            Querystring
        }

        protected enum AuthenticationTypes
        {
            Header,
            PostData,
            PostForm
        }

        private ClientConfig _configuration;
        protected ClientConfig Configuration { get { return _configuration; } set { _configuration = value; } }

        private readonly WebRequest2 _webRequest = new WebRequest2();

        public ClientTypes ProviderName { get; set; }

        #region EndPoints
        protected string AccessCodeServiceEndpoint { get; set; }
        protected string AccessTokenServiceEndpoint { get; set; }
        protected string UserInfoServiceEndpoint { get; set; }
        #endregion

        private ResponseTypes _responseType = ResponseTypes.Json;
        protected ResponseTypes ResponseType { get { return _responseType; } set { _responseType = value; } }

        private AuthenticationTypes _authenticationType = AuthenticationTypes.PostData;
        protected AuthenticationTypes AuthenticationType { get { return _authenticationType; } set { _authenticationType = value; } }

        #region Keywords
        private string _clientIdKeyword = "client_id";
        protected string ClientIdKeyword { get { return _clientIdKeyword; } set { _clientIdKeyword = value; } }

        private string _clientSecretKeyword = "client_secret";
        protected string ClientSecretKeyword { get { return _clientSecretKeyword; } set { _clientSecretKeyword = value; } }

        private string _scopeKeyword = "scope";
        protected string ScopeKeyword { get { return _scopeKeyword; } set { _scopeKeyword = value; } }

        private string _accessTokenKeyword = "access_token";
        protected string AccessTokenKeyword { get { return _accessTokenKeyword; } set { _accessTokenKeyword = value; } }

        private string _grantType = "authorization_code";
        protected string GrantType { get { return _grantType; } set { _grantType = value; } }

        #endregion

        public string DefaultScope { get; set; }

        protected OAuth2Client(ClientConfig config)
        {
            _configuration = config;

            if (!_configuration.Scope.IsEmpty())
                DefaultScope = _configuration.Scope;
        }

        public void ChangeAccessTokenServiceEndpoint(string url)
        {
            AccessTokenServiceEndpoint = url;
        }

        // 1
        public virtual string GetAccessCodeUrl(AuthenticationResponseType responseType, string redirectUrl, List<string> state, Dictionary<string, string> extraData)
        {
            var dic = new Dictionary<string, string>
            {
                {"response_type", responseType == AuthenticationResponseType.Code ? "code" : "token"},
                {ClientIdKeyword, _configuration.ClientId}
            };
            if (!redirectUrl.IsEmpty())
                dic.Add("redirect_uri", HttpUtility.UrlEncode(redirectUrl));

            dic.Add(ScopeKeyword, _configuration.Scope.IsEmpty() ? DefaultScope : _configuration.Scope);
            if (extraData != null && extraData.Count > 0)
            {
                foreach (var key in extraData.Keys)
                    dic.Add(key, extraData[key]);
            }

            if (state != null && state.Count > 0)
            {
                var st = "";
                foreach (var s in state)
                {
                    if (!string.IsNullOrWhiteSpace(s))
                        st += "|";
                    st += s;
                }
                dic.Add("state", st);
            }

            var url = AccessCodeServiceEndpoint;
            url += url.Contains("?") ? "&" : "?";
            url += dic.ToEncode();
            return url.Replace("%2520", "%20");
        }

        // 2
        public virtual AuthenticationToken GetAuthenticationToken(string code, string redirectUrl)
        {
            var dic = new Dictionary<string, string>();
            if (!code.IsEmpty())
                dic.Add("code", code);
            dic.Add(ClientIdKeyword, _configuration.ClientId);
            dic.Add("grant_type", GrantType);
            if (!_configuration.ClientSecret.IsEmpty())
                dic.Add(ClientSecretKeyword, _configuration.ClientSecret);
            if (!redirectUrl.IsEmpty())
                dic.Add("redirect_uri", redirectUrl);
            var response = _webRequest.PostForm(AccessTokenServiceEndpoint, dic);

            var tokenInfo = new AuthenticationToken();
            try
            {
                if (ResponseType == ResponseTypes.Json)
                    tokenInfo = response.ParseJson<AuthenticationToken>();
                else
                {
                    var query = HttpUtility.ParseQueryString(response);
                    if (query["error"] != null)
                        tokenInfo.Error = query["error"];
                    if (query["error_description"] != null)
                        tokenInfo.ErrorDesc = query["error_description"];
                    if (query["access_token"] != null)
                        tokenInfo.AccessToken = query["access_token"];
                    if (query["refresh_token"] != null)
                        tokenInfo.RefreshToken = query["refresh_token"];
                    if (query["token_type"] != null)
                        tokenInfo.TokenType = query["token_type"];
                    if (query["expires_in"] != null)
                        tokenInfo.Expires = int.Parse(query["expires_in"]);
                }
                return tokenInfo;
            }
            catch (Exception ex)
            {
                tokenInfo.Error = "HATA";
                tokenInfo.ErrorDesc = Logger.GetErrorMessage(ex);
            }
            return tokenInfo;
        }

        // 2
        public virtual AuthenticationToken GetRefreshToken(string refreshToken)
        {
            var dic = new Dictionary<string, string>
            {
                {ClientIdKeyword, _configuration.ClientId},
                {"grant_type", "refresh_token"}
            };
            if (!_configuration.ClientSecret.IsEmpty())
                dic.Add(ClientSecretKeyword, _configuration.ClientSecret);
            dic.Add("refresh_token", refreshToken);
            var response = _webRequest.PostForm(AccessTokenServiceEndpoint, dic);

            var tokenInfo = new AuthenticationToken();
            try
            {
                if (ResponseType == ResponseTypes.Json)
                {
                    tokenInfo = response.ParseJson<AuthenticationToken>();
                }
                else
                {
                    var query = HttpUtility.ParseQueryString(response);
                    if (query["error"] != null)
                        tokenInfo.Error = query["error"];
                    if (query["error_description"] != null)
                        tokenInfo.ErrorDesc = query["error_description"];
                    if (query["access_token"] != null)
                        tokenInfo.AccessToken = query["access_token"];
                    if (query["refresh_token"] != null)
                        tokenInfo.RefreshToken = query["refresh_token"];
                    if (query["token_type"] != null)
                        tokenInfo.TokenType = query["token_type"];
                    if (query["expires_in"] != null)
                        tokenInfo.Expires = int.Parse(query["expires_in"]);
                }
                return tokenInfo;
            }
            catch (Exception ex)
            {
                tokenInfo.Error = "HATA";
                tokenInfo.ErrorDesc = Logger.GetErrorMessage(ex);
            }
            return tokenInfo;
        }


        private Dictionary<string, string> _userInfoData;
        private Dictionary<string, string> _userInfoHeader;
        public T GetUserInfo<T>(string accessToken, string tokenType = null, Dictionary<string, string> additionalData = null)
            where T : class, new()
        {
            var resultContent = UserData(accessToken, tokenType, additionalData);
            return resultContent.ParseJson<T>();
        }

        //public dynamic GetUserInfo(string accessToken, string tokenType = null, Dictionary<string, string> additionalData = null)
        //{
        //    var resultContent = UserData(accessToken, tokenType, additionalData);

        //    dynamic json = JObject.Parse(resultContent);
        //    return json;
        //}

        public string GetUserInfo2(string accessToken, string tokenType = null, Dictionary<string, string> additionalData = null)
        {
            var resultContent = UserData(accessToken, tokenType, additionalData);
            return resultContent;
        }

        private string UserData(string accessToken, string tokenType = null, Dictionary<string, string> additionalData = null)
        {
            var url = UserInfoServiceEndpoint;

            _userInfoData = new Dictionary<string, string>();
            if (AccessTokenKeyword.IsEmpty())
                throw new Exception("UserInfoTokenKeyword must be set!");
            if (accessToken.IsEmpty())
                throw new Exception("AccessToken is null");

            if (AuthenticationType == AuthenticationTypes.PostData)
                _userInfoData.Add(AccessTokenKeyword, accessToken);

            else if (AuthenticationType == AuthenticationTypes.PostForm)
                _userInfoData.Add(AccessTokenKeyword, accessToken);

            if (additionalData != null)
            {
                foreach (var a in additionalData)
                    _userInfoData.Add(a.Key, a.Value);
            }

            _userInfoHeader = new Dictionary<string, string>();
            BeforeGetUserInfo(accessToken, _userInfoData, _userInfoHeader);

            if (AuthenticationType == AuthenticationTypes.PostData)
            {
                return _webRequest.GetUrl(url, _userInfoData, _userInfoHeader);
            }
            else if (AuthenticationType == AuthenticationTypes.PostForm)
            {
                return _webRequest.PostForm(url, _userInfoData);
            }
            else if (AuthenticationType == AuthenticationTypes.Header)
            {
                return _webRequest.GetUrl(url, _userInfoData, _userInfoHeader, accessToken, tokenType);
            }
            return null;
        }

        protected virtual void BeforeGetUserInfo(string accessToken, Dictionary<string, string> data, Dictionary<string, string> header)
        {
            _userInfoData = data;
            _userInfoHeader = header;
        }
    }
}