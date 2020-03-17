using System;
using System.Web;

namespace MFramework.Web.Common.Helpers
{
    public interface ICacheHelperBase
    {
        T Get<T>(string key) where T : class;
        void Set<T>(string key, T obj, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null);
        void Remove(string key);
    }

    public partial class CacheHelperBase : ICacheHelperBase
    {
        public T Get<T>(string key) where T : class
        {
            if (HttpContext.Current.Cache.Get(key) == null) return default(T);
            return HttpContext.Current.Cache.Get(key) as T;
        }

        public void Set<T>(string key, T obj, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            DateTime noAbsoluteExp = System.Web.Caching.Cache.NoAbsoluteExpiration;
            TimeSpan noSlidingExp = System.Web.Caching.Cache.NoSlidingExpiration;

            HttpContext.Current.Cache.Insert(key, obj, null, absoluteExpiration ?? noAbsoluteExp, slidingExpiration ?? noSlidingExp);
        }

        public void Remove(string key)
        {
            if (HttpContext.Current.Cache.Get(key) == null) HttpContext.Current.Cache.Remove(key);
        }
    }
}