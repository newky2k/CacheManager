using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DSoft.CacheManager
{
    public interface IStorageBackend
    {
        LiteDatabase Database { get; }

        /// <summary>
        /// Does the cache exist in the storage backend
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        bool CacheEntryExists(string keyName);

        IList<T> GetItems<T>(string keyName);

    }
}
