using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace MFramework.Web.Common.Helpers
{
    public interface IRestHelperBase
    {
        IRestResponse ExecuteNonAuthorize(IRestRequest restRequest);
        IRestResponse<T> ExecuteNonAuthorize<T>(IRestRequest restRequest);
        IRestResponse ExecuteWithCustomAuthorize(IRestRequest restRequest, IAuthenticator authenticator);
        IRestResponse<T> ExecuteWithCustomAuthorize<T>(IRestRequest restRequest, IAuthenticator authenticator);
        IRestResponse ExecuteWithDefaultAuthorize(IRestRequest restRequest);
        IRestResponse<T> ExecuteWithDefaultAuthorize<T>(IRestRequest restRequest);
    }

    public partial class RestHelperBase : IRestHelperBase
    {
        public readonly RestClient Client;
        public readonly ICacheHelperBase CacheHelper;
        public readonly IConfigHelperBase ConfigHelper;

        public RestHelperBase(ICacheHelperBase cacheHelper, IConfigHelperBase configHelper)
        {
            CacheHelper = cacheHelper;
            ConfigHelper = configHelper;

            Client = new RestClient(ConfigHelper.ApiUrl);
            Settings();
        }

        public RestHelperBase(string url, ICacheHelperBase cacheHelper, IConfigHelperBase configHelper)
        {
            CacheHelper = cacheHelper;
            ConfigHelper = configHelper;

            Client = new RestClient(url);
            Settings();
        }

        private void Settings()
        {
            Client.AddHandler("application/json", () => new NewtonsoftJsonSerializer());
            Client.AddHandler("text/json", () => new NewtonsoftJsonSerializer());
        }

        public IRestResponse ExecuteWithDefaultAuthorize(IRestRequest restRequest)
        {
            Client.Authenticator = new JwtAuthenticator(GetToken());
            return Client.Execute(restRequest);
        }

        public IRestResponse<T> ExecuteWithDefaultAuthorize<T>(IRestRequest restRequest)
        {
            Client.Authenticator = new JwtAuthenticator(GetToken());
            return Client.Execute<T>(restRequest);
        }

        public IRestResponse ExecuteWithCustomAuthorize(IRestRequest restRequest, IAuthenticator authenticator)
        {
            Client.Authenticator = authenticator;
            return Client.Execute(restRequest);
        }

        public IRestResponse<T> ExecuteWithCustomAuthorize<T>(IRestRequest restRequest, IAuthenticator authenticator)
        {
            Client.Authenticator = authenticator;
            return Client.Execute<T>(restRequest);
        }

        public IRestResponse ExecuteNonAuthorize(IRestRequest restRequest)
        {
            Client.Authenticator = null;
            return Client.Execute(restRequest);
        }

        public IRestResponse<T> ExecuteNonAuthorize<T>(IRestRequest restRequest)
        {
            Client.Authenticator = null;
            return Client.Execute<T>(restRequest);
        }

        private string GetToken()
        {
            string token = GetAccessTokenFromCache();

            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
            {
                token = RequestDefaultAccessToken();
                SetAccessTokenToCache(token);
            }

            return token;
        }

        private string RequestDefaultAccessToken()
        {
            RestRequest request = new RestRequest(ConfigHelper.ApiAuthUrl, Method.POST);
            request.AddJsonBody(new AuthenticateModel { Email = ConfigHelper.ApiUid, Password = ConfigHelper.ApiPass });
            IRestResponse<DefaultApiLogOnUser> response = ExecuteNonAuthorize<DefaultApiLogOnUser>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (response != null && response.Data != null)
                {
                    DefaultApiLogOnUser apiLogOnUser = response.Data;
                    return apiLogOnUser.Token;
                }
            }

            return string.Empty;
        }


        private string GetAccessTokenFromCache()
        {
            string token = CacheHelper.Get<string>("access_token");

            if (string.IsNullOrEmpty(token) == false)
            {
                JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

                if (jwtToken.ValidTo < DateTime.Now)
                {
                    CacheHelper.Remove("access_token");
                    token = string.Empty;
                }
            }

            return token;
        }

        private void SetAccessTokenToCache(string token)
        {
            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            DateTime absoluteExp = jwtToken.ValidTo.AddHours(-12) < DateTime.Now ? jwtToken.ValidTo : jwtToken.ValidTo.AddHours(-12);

            CacheHelper.Set("access_token", token, absoluteExp);
        }
    }

    public partial class NewtonsoftJsonSerializer : ISerializer, IDeserializer
    {
        public string ContentType { get; set; } = "application/json";

        public string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(IRestResponse response)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response.Content);
        }
    }

    public partial class AuthenticateModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public partial class DefaultApiLogOnUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string Token { get; set; }
        public int EmployeeType { get; set; }
        public string EmployeeCode { get; set; }
    }
}