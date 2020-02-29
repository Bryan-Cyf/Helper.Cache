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
                return MemoryCache.Default;//�̰߳�ȫ
            }
        }

        /// <summary>
        /// ��ȡ����
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
        /// ���ӻ���
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime">����</param>
        public void Add(string key, object data, int cacheTime = 30)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Cache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return Cache.Contains(key);
        }

        public int Count { get { return (int)(Cache.GetCount()); } }


        /// <summary>
        /// �������
        /// </summary>
        /// <param name="key">/key</param>
        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// ������ʽ�Ƴ�
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
        /// ���ݼ�ֵ���ػ�������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return Cache.Get(key); }
            set { Add(key, value); }
        }

        /// <summary>
        /// ���ȫ������
        /// </summary>
        public void RemoveAll()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }
    }
}