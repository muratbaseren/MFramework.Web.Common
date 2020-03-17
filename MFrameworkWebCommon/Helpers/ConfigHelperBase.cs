using System;
using System.Configuration;

namespace MFramework.Web.Common.Helpers
{
    public interface IConfigHelperBase
    {
        string ApiAuthUrl { get; set; }
        string ApiPass { get; set; }
        string ApiUid { get; set; }
        string ApiUrl { get; set; }

    }

    public static class ConfigNames
    {
        public static string ApiUid = "ApiUid";
        public static string ApiPass = "ApiPass";
        public static string ApiUrl = "ApiUrl";
        public static string ApiAuthUrl = "ApiAuthUrl";
    }

    public partial class ConfigHelperBase : IConfigHelperBase
    {
        public string ApiUid { get; set; }
        public string ApiPass { get; set; }
        public string ApiUrl { get; set; }
        public string ApiAuthUrl { get; set; }

        public ConfigHelperBase()
        {
            if (ConfigurationManager.AppSettings.HasKeys())
            {
                ApiUid = ConfigurationManager.AppSettings[ConfigNames.ApiUid];
                ApiPass = ConfigurationManager.AppSettings[ConfigNames.ApiPass];
                ApiUrl = ConfigurationManager.AppSettings[ConfigNames.ApiUrl];
                ApiAuthUrl = ConfigurationManager.AppSettings[ConfigNames.ApiAuthUrl];
            }
        }
    }
}