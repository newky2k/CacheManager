using System;
using System.Collections.Generic;
using System.Text;

namespace DSoft.CacheManager.LiteDB
{
    public class LiteDbStorageOptions
    {
        /// <summary>
        /// Gets or sets the location for the cache file to be stored
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the name of the cache file
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the password for encrypting the cache file
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
    }
}
