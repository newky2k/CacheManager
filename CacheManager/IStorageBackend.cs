using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DSoft.CacheManager
{
    public interface IStorageBackend
    {
        /// <summary>
        /// Does the cache exist in the storage backend
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        bool CacheEntryExists(string keyName);

        IList<T> GetItems<T>(string keyName);

        T Find<T>(string keyName, Expression<Func<T, bool>> predicate);

        void Update<T>(string keyName, T data);

        void Insert<T>(string keyName, T data);

        void EnsureIndexed<T, K>(string keyName, Expression<Func<T, K>> keySelector);

        void SetItems<T>(string keyName, List<T> data);

    }
}
