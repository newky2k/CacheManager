using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DSoft.CacheManager
{
    public class CacheManager : ICacheManager
    {
        private const string CacheItemCollectionName = "CacheItems";

        #region Fields
        private CacheManagerItemCollection _cachedItems;
        private bool _isLoaded;
        private Dictionary<string, object> _dataDictionary = new Dictionary<string, object>();
        private object getItemLock = new object();
        private ICacheStorageBackend _backend;

        #endregion

        #region Properties

        #region Private Properties

        private CacheManagerItemCollection Cache
        {
            get
            {
                if (_cachedItems == null)
                {
                    _cachedItems = LoadCache();

                    _isLoaded = true;
                }


                return _cachedItems;
            }
            set
            {
                _cachedItems = value;
            }
        }


        private ICacheStorageBackend BackEnd
        {
            get
            {
                if (_backend == null)
                    throw new Exception("No backend has been provided");
                    

                return _backend;
            }
        }

        #endregion

        #endregion

        #region Constructors

        public CacheManager(ICacheStorageBackend backEndProvider)
        {
            _backend = backEndProvider;

            _backend.Prepare();
        }


        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified key is registered
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if  specified key is registered; otherwise, <c>false</c>.
        /// </returns>
        public bool IsKeyRegistered(string key) => Cache.ContainsKey(key);

        /// <summary>
        /// Gets the items from the cache for the specified cache key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The cache key</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// No data registered with key: {key} in the CacheManager
        /// or
        /// No data registered with key: {key} in the CacheManager
        /// </exception>
        public List<T> GetItems<T>(string key)
        {
            if (!Cache.ContainsKey(key))
                throw new Exception($"No data registered with key: {key} in the CacheManager");

            if (!_dataDictionary.ContainsKey(key) || _dataDictionary[key] == null)
            {
                lock (getItemLock) //handle double loading
                {
                    //need to load the data from the database
                    if (!BackEnd.CacheEntryExists(key))
                        throw new Exception($"No data registered with key: {key} in the CacheManager");

                    if (_dataDictionary.ContainsKey(key) && _dataDictionary[key] != null)
                        return (List<T>)_dataDictionary[key]; //handle check after the first lock has completed

                    var data = BackEnd.GetItems<T>(key);

                    if (_dataDictionary.ContainsKey(key))
                        _dataDictionary[key] = data;
                    else
                        _dataDictionary.Add(key, data);
                }

            }

            return (List<T>)_dataDictionary[key];
        }

        /// <summary>
        /// Set the cached items for the specified cache key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="content">The content.</param>
        /// <param name="lastUpdated">The last updated.</param>
        public void SetItems<T>(string key, List<T> content, DateTime? lastUpdated = null)
        {
             var dTime = lastUpdated.HasValue ? lastUpdated : DateTime.Now;

            if (!Cache.ContainsKey(key))
            {
                var newItem = AddNewKey<T>(key, dTime.Value);

                Cache.Add(newItem);
            }
            else
            {
                //replace
                Cache[key].LastUpdated = dTime;
            }

            if (_dataDictionary.ContainsKey(key))
                _dataDictionary[key] = content;
            else
                _dataDictionary.Add(key, content);

            UpdateStoredCache(key, content, dTime.Value);

        }

        /// <summary>
        /// Gets the last updated timestamp for the specified cache key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="Exception">No data registered with key: {key} in the CacheManager</exception>
        public DateTime? GetLastUpdated(string key)
        {
            if (!Cache.ContainsKey(key))
                throw new Exception($"No data registered with key: {key} in the CacheManager");

            return Cache[key].LastUpdated;

        }

        /// <summary>
        /// Updates the cached item last updated timestamp, for the specified cache key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="lastUpdated">The last updated.</param>
        /// <exception cref="Exception">No data registered with key: {key} in the CacheManager</exception>
        public void UpdateContentsLastUpdated<T>(string key, DateTime? lastUpdated = null)
        {
            if (!Cache.ContainsKey(key))
                throw new Exception($"No data registered with key: {key} in the CacheManager");

            var dTime = (lastUpdated.HasValue) ? lastUpdated : DateTime.Now;

            Cache[key].LastUpdated = dTime;

            UpdateCacheKeys<T>(key, dTime.Value);
        }


        /// <summary>
        /// Loads the cache asynchronously.
        /// </summary>
        public async Task LoadAsync()
        {
            await Task.Run(() =>
            {
                Cache = LoadCache();

                _isLoaded = true;
            });

        }

        /// <summary>
        /// Syncronises the changes to the database, asynchronously.
        /// </summary>
        public async Task SyncroniseAsync()
        {
            await Task.Run(() =>
            {
                SaveCache();
            });

        }

        public void Synronise()
        {
            Task.Factory.StartNew(() =>
            {
                SaveCache();
            });

        }

        /// <summary>
        /// Resets the cache
        /// </summary>
        public void ResetCache() => BackEnd.Reset();

        #endregion

        #region Private Methods

        private CacheManagerItem AddNewKey<T>(string key, DateTime updateTime)
        {
            var existingItem = BackEnd.Find<CacheManagerItem>(CacheItemCollectionName, x => x.Key.Equals(key));

            if (existingItem != null)
            {
                existingItem.LastUpdated = updateTime;
                existingItem.Type = typeof(T).AssemblyQualifiedName;
                existingItem.ListType = typeof(List<T>).AssemblyQualifiedName;

                BackEnd.Update(CacheItemCollectionName, existingItem);
            }
            else
            {
                existingItem = new CacheManagerItem()
                {
                    Key = key,
                    LastUpdated = updateTime,
                    Type = typeof(T).AssemblyQualifiedName,
                    ListType = typeof(List<T>).AssemblyQualifiedName,
                };


                BackEnd.Insert(CacheItemCollectionName, existingItem);
            }

            BackEnd.EnsureIndexed<CacheManagerItem, string>(CacheItemCollectionName, x => x.Key);

            return existingItem;

        }

        private void UpdateCacheKeys<T>(string key, DateTime updateTime)
        {
            var existingItem = BackEnd.Find<CacheManagerItem>(CacheItemCollectionName, x => x.Key.Equals(key));

            if (existingItem != null)
            {
                existingItem.LastUpdated = updateTime;

                BackEnd.Update(CacheItemCollectionName, existingItem);
            }
            else
            {
                var newKey = new CacheManagerItem()
                {
                    Key = key,
                    LastUpdated = updateTime,
                    Type = typeof(T).AssemblyQualifiedName,
                    ListType = typeof(List<T>).AssemblyQualifiedName,
                };

                BackEnd.Insert(CacheItemCollectionName, existingItem);
            }

            BackEnd.EnsureIndexed<CacheManagerItem, string>(CacheItemCollectionName, x => x.Key);
        }

        private void UpdateStoredCache<T>(string key, List<T> content, DateTime updateTime)
        {
            UpdateCacheKeys<T>(key, updateTime);

            BackEnd.SetItems<T>(key, content);

        }

        private CacheManagerItemCollection LoadCache()
        {
            try
            {

                
                var items = new CacheManagerItemCollection();

                if (!BackEnd.Exists)
                    return items;

                //load the keys into memory, other items can be loaded as an when
                LoadKeys(items);


                //try loading the stored data too
                foreach (var aKey in items)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(aKey.Type))
                        {
                            var aType = Type.GetType(aKey.Type);

                            var ex = typeof(ICacheStorageBackend);
                            var mi = ex.GetMethod(nameof(ICacheStorageBackend.GetItems));
                            var miConstructed = mi.MakeGenericMethod(aType);
                            
                            var dList = (IList)miConstructed.Invoke(BackEnd, new object[] { aKey.Key });

                            ////old way?
                            //var dList = (IList)Activator.CreateInstance(dType);

                            //var count = dList.Count;

                            //var col = BackEnd.Database.GetCollection(aKey.Key);

                            //var mapper = BsonMapper.Global;

                            //foreach (var aItem in col.FindAll())
                            //{
                            //    var newItem = mapper.ToObject(Type.GetType(aKey.Type), aItem);

                            //    dList.Add(newItem);
                            //}

                            if (_dataDictionary.ContainsKey(aKey.Key))
                                _dataDictionary[aKey.Key] = dList;
                            else
                                _dataDictionary.Add(aKey.Key, dList);


                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);

                    }


                }


                return items;


            }
            catch (Exception)
            {

                return new CacheManagerItemCollection();
            }


        }

        private void LoadKeys(CacheManagerItemCollection tree)
        {
            var allKeys = BackEnd.GetItems<CacheManagerItem>(CacheItemCollectionName);

            tree.AddRange(allKeys);

        }

        private void SaveCache()
        {
            try
            {
                //LiteDb saves on update
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
        }

        public void Dispose()
        {
            _backend = null;
        }

        #endregion

    }
}
