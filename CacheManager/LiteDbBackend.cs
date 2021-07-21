using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DSoft.CacheManager.LiteDB
{
    public class LiteDbBackend : IStorageBackend
    {
        private LiteDatabase _liteDatabase = null;

        public string Location { get; set; }

        public string FileName { get; set; }

        public string Password { get; set; }

        private string StoragePath => Path.Combine(Location, FileName);

        private ConnectionString DatabaseConnectionString => new ConnectionString($"Filename={StoragePath};Password={Password}");

        public LiteDatabase Database
        {
            get
            {
                if (_liteDatabase == null)
                    _liteDatabase = new LiteDatabase(DatabaseConnectionString);

                return _liteDatabase;
            }
        }

        public bool CacheEntryExists(string keyName) => Database.CollectionExists(keyName);

        public IList<T> GetItems<T>(string keyName)
        {
            if (!CacheEntryExists(keyName))
                return new List<T>();

            var aCol = Database.GetCollection<T>(keyName);

            if (aCol.Count() == 0)
                return new List<T>();

            return aCol.FindAll().ToList();
        }
    }
}
