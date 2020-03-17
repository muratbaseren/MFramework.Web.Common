using Autofac;

namespace MFramework.Web.Common
{
    public partial class AppScope 
    {
#if DEBUG
        public static string Environment { get; set; } = "dev";
#else
        public static string Environment { get; set; } = "prod";
#endif
        public static IContainer Container { get; set; }
    }
}