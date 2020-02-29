using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Common
{
    /// <summary>
    /// MemoryCacheCache 
    /// </summary>
    public class MemoryCacheCache : ICache
    {
        public MemoryCacheCache() { }

        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;//线程安全
            }
        }

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (Cache.Contains(key))
            {
                return (T)Cache[key];
            }
            else
            {
                return default(T);
            }
        }

        public object Get(string key)
        {
            return Cache[key];
        }

        /// <summary>
        /// 增加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime">分钟</param>
        public void Add(string key, object data, int cacheTime = 30)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Cache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return Cache.Contains(key);
        }

        public int Count { get { return (int)(Cache.GetCount()); } }


        /// <summary>
        /// 单个清除
        /// </summary>
        /// <param name="key">/key</param>
        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// 正则表达式移除
        /// </summary>
        /// <param name="pattern">pattern</param>
        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach (var item in Cache)
                if (regex.IsMatch(item.Key))
                    keysToRemove.Add(item.Key);

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// 根据键值返回缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return Cache.Get(key); }
            set { Add(key, value); }
        }

        /// <summary>
        /// 清除全部数据
        /// </summary>
        public void RemoveAll()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }
    }
}