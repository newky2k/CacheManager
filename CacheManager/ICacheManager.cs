using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSoft.CacheManager
{
    public interface ICacheManager : IDisposable
    {
        bool IsKeyRegistered(string key);

        List<T> GetItems<T>(string key);

        void SetItems<T>(string key, List<T> content, DateTime? lastUpdated = null);

        DateTime? GetLastUpdated(string key);

        void UpdateContentsLastUpdated<T>(string key, DateTime? lastUpdated = null);

        Task SyncroniseAsync();

        void Synronise();

        Task LoadAsync();

        void ResetCache();

    }
}
