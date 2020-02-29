using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CacheHelper
    {
        private static readonly Object _locker = new object();
        public static T GetCache<T>(string key)
        {
            try
            {
                lock (_locker)
                {
                    return (T)MemoryCache.Default[key];
                }
            }
            catch (Exception ex)
            {
                //LoggerHelper.LogError(ex);
                return default(T);
            }
        }
        public static T GetCache<T>(String key, Func<T> cachePopulate, DateTime? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("Invalid cache key");
                if (cachePopulate == null) throw new ArgumentNullException("cachePopulate");
                if (slidingExpiration == null && absoluteExpiration == null) throw new ArgumentException("Either a sliding expiration or absolute must be provided");

                if (MemoryCache.Default[key] == null)
                {
                    var data = cachePopulate();
                    if (data == null) return default(T);
                    if (absoluteExpiration == null)
                    {
                        absoluteExpiration = DateTime.Now.Add(TimeSpan.FromHours(1));
                    }
                    //var value = cachePopulate();
                    var item = new CacheItem(key, data);
                    var policy = CreatePolicy(slidingExpiration, absoluteExpiration);
                    MemoryCache.Default.Add(item, policy);
                }
                var ret = (T)MemoryCache.Default[key];
                return ret;
            }
            catch (Exception ex)
            {
                //LoggerHelper.LogError(ex);
                return default(T);
            }
        }

        public static bool SetCache(String key, object obj, TimeSpan? slidingExpiration = null, DateTime? absoluteExpiration = null)
        {
            try
            {
                if (absoluteExpiration == null)
                {
                    absoluteExpiration = DateTime.Now.Add(TimeSpan.FromHours(1));
                }
                Remove(key);
                var item = new CacheItem(key, obj);
                var policy = CreatePolicy(slidingExpiration, absoluteExpiration);
                return MemoryCache.Default.Add(item, policy);
            }
            catch (Exception ex)
            {
                //LoggerHelper.LogError(ex);
                return false;
            }
        }
        public static bool SetCache<T>(String key, Func<T> cachePopulate, TimeSpan? slidingExpiration = null, DateTime? absoluteExpiration = null)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("Invalid cache key");
                if (cachePopulate == null) throw new ArgumentNullException("cachePopulate");
                if (slidingExpiration == null && absoluteExpiration == null) throw new ArgumentException("Either a sliding expiration or absolute must be provided");

                try
                {
                    var value = cachePopulate();
                    Remove(key);
                    return SetCache(key, cachePopulate(), slidingExpiration, absoluteExpiration);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return false;
                }
            }
            catch (Exception ex)
            {
                //LoggerHelper.LogError(ex);
                return false;
            }
        }

        public static object Remove(string key)
        {
            try
            {
                return MemoryCache.Default.Remove(key);
            }
            catch (Exception ex)
            {
                //LoggerHelper.LogError(ex);
                return null;
            }
        }

        private static CacheItemPolicy CreatePolicy(TimeSpan? slidingExpiration, DateTime? absoluteExpiration)
        {
            try
            {
                var policy = new CacheItemPolicy();
                if (absoluteExpiration.HasValue)
                {
                    policy.AbsoluteExpiration = absoluteExpiration.Value;
                }
                else if (slidingExpiration.HasValue)
                {
                    policy.SlidingExpiration = slidingExpiration.Value;
                }
                policy.Priority = CacheItemPriority.Default;
                return policy;
            }
            catch (Exception ex)
            {
                //LoggerHelper.LogError(ex);
                return null;
            }
        }
    }

}
