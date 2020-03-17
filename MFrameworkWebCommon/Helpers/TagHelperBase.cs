
namespace MFramework.Web.Common.Helpers
{
    public interface ITagHelperBase
    {
        //string GenerateLinkByPage(this UrlHelper helper, string idWithinPage = "#top");
    }

    public partial class TagHelperBase : ITagHelperBase
    {
        //public static string GenerateLinkByPage(this UrlHelper helper, string idWithinPage = "#top")
        //{
        //    return HttpContext.Current.Request.Url.AbsolutePath == RouteHelper.HomeRoute
        //        ? idWithinPage
        //        : RouteHelper.HomeRoute + idWithinPage;
        //}
    }
}