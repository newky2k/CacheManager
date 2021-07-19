using System;
using System.Collections.Generic;
using System.Text;

namespace DSoft.CacheManager
{
    internal class CacheManagerItem
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Type { get; set; }

        public string ListType { get; set; }

        public DateTime? LastUpdated { get; set; }

    }
}
